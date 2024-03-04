using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Model;
using Architecture.Reducers;
using Architecture.Services;
using DG.Tweening;
using DragonBones;
using Helper;
using Models;
using Newtonsoft.Json.Linq;
using Tiled2Unity;
using UIComponents.Map;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Components
{
    public class MapManager : MonoBehaviour
    {
        public TiledMap TiledMap;
        public UnityArmatureComponent ModelCharacter;
        public UnityArmatureComponent ModelPoint;
        public BoxCollider2D MapCollider;

        private float _tileWidthOnScreen;
        private float _tileHeightOnScreen;
        private Maze _maze;
        private Tweener _tweenMove;

        [SerializeField] private GameObject _pfBlock;
        [SerializeField] private GameObject _pfItem;
        [SerializeField] private GameObject _pfCampaignGate;
        [SerializeField] private GameObject _goMap;
        [SerializeField] private GameObject _pfGainItemEffect;

        private List<MapItem> _lstMapItems;


        private void Awake()
        {
            Map.CleanActionCreator();
            _lstMapItems = new List<MapItem>();
            DisplayModelPoint(false);


            JObject roadData = Ultilities.GetData("Data/road");
            //SETUP MAP
            _tileWidthOnScreen = roadData.Value<float>("tW");
            _tileHeightOnScreen = roadData.Value<float>("tH");
            float mapWidthInSpace = _tileWidthOnScreen * TiledMap.NumTilesWide;
            float mapHeightInSpace = _tileHeightOnScreen * TiledMap.NumTilesHigh;
            MapCollider.offset = new Vector2(mapWidthInSpace / 2, -mapHeightInSpace / 2);
            MapCollider.size = new Vector2(mapWidthInSpace, mapHeightInSpace);


            //            //BLOCKS
            //            JObject blocksData = Ultilities.GetData("Data/block");
            //            List<PositionModel> blocks = blocksData.Value<JArray>("data").ToObject<List<PositionModel>>();
            //            InitBlock(blocks);

            //ROAD
            List<JObject> road = roadData.Value<JArray>("data").ToObject<List<JObject>>();
            InitMaze(road);

            //Init positionx
            JObject campaignData = Ultilities.GetData("Data/campaign");
            List<JObject> campaign = campaignData.Value<JArray>("data").ToObject<List<JObject>>();
            InitCampaignGate(campaign);


            Redux.Subject
                .StartWith(Redux.State)
                .Subscribe(state =>
                {
                    MovePoint firstPosition = state.MapState.CurrentPosition;
                    if (firstPosition != null && !_isInitPosition)
                    {
                        InitPosition(firstPosition.X, firstPosition.Y);
                    }

                    List<MapItemModel> lstItems = state.MapState.Items;
                    if (Ultilities.IsEmpty(lstItems) || _isInitItem) return;
                    InitItem(lstItems);
                })
                .AddTo(this);
        }

        private bool _isInitItem;
        private bool _isInitPosition;

        private List<PositionModel> _lstBlocks;

        private void InitBlock(List<PositionModel> lstBlocks)
        {
            this._lstBlocks = lstBlocks;
            for (int i = 0; i < lstBlocks.Count; i++)
            {
                PositionModel block = lstBlocks[i];
                GameObject goBlock = Instantiate(_pfBlock, _goMap.transform);

                int x = block.X;
                int y = block.Y;
                Vector3 position = GetLocalPosition(-x, -(y + 1));
                position.x += _tileWidthOnScreen / 2;
                position.y += _tileHeightOnScreen / 2;
                goBlock.transform.localPosition = position;
            }
        }

        private void InitPosition(int x, int y)
        {
            _isInitPosition = true;
            SetPosition(x, y);
        }

        private void InitMaze(List<JObject> road, List<PositionModel> blocks = null)
        {
            //CREATE MAZE.
            int[,] map = new int[TiledMap.NumTilesWide, TiledMap.NumTilesHigh];
            for (int i = 0; i < road.Count; i++)
            {
                JObject roadObj = road[i];
                int x = roadObj.Value<int>("x");
                int y = roadObj.Value<int>("y");
                map[x, y] = 1;
            }

            //            for (int i = 0; i < blocks.Count; i++)
            //            {
            //                PositionModel block = blocks[i];
            //                map[block.X, block.Y] = 0;
            //            }

            _maze = new Maze(TiledMap.NumTilesWide, TiledMap.NumTilesHigh, map);
        }

        private void InitCampaignGate(List<JObject> campaign)
        {
            for (int i = 0; i < campaign.Count; i++)
            {
                JObject campaignObj = campaign[i];
                float x = campaignObj.Value<float>("x");
                float y = campaignObj.Value<float>("y");
                int regionId = campaignObj.Value<int>("rId");

                GameObject goGate = Instantiate(_pfCampaignGate, _goMap.transform);
                Vector3 pos = GetLocalPosition(-x, -y);
                pos.x += 0.3f;
                pos.y -= 0.17f;

                goGate.transform.localPosition = pos;

                MapCampaignGate mapCampaignGate = goGate.GetComponent<MapCampaignGate>();
                mapCampaignGate.RegionId = regionId;
                mapCampaignGate.X = x;
                mapCampaignGate.Y = y;
            }
        }

        private void InitItem(List<MapItemModel> lstItems)
        {
            _isInitItem = true;
            for (int i = 0; i < lstItems.Count; i++)
            {
                MapItemModel item = lstItems[i];
                GameObject goItem = Instantiate(_pfItem, _goMap.transform);

                int x = item.X;
                int y = item.Y;
                Vector3 position = GetLocalPosition(-x, -(y + 1));
                position.x += _tileWidthOnScreen / 2;
                position.y += _tileHeightOnScreen / 2;
                goItem.transform.localPosition = position;

                MapItem mapItem = goItem.GetComponent<MapItem>();
                mapItem.Init(item.Type, x, y);

                _lstMapItems.Add(mapItem);
            }
        }

        private void DisplayModelPoint(bool isDisplay)
        {
            ModelPoint.gameObject.SetActive(isDisplay);
        }

        private List<MovePoint> _currentRoad;
        private int _currentRoadId;
        private readonly MovePoint _currentPosition = MovePoint.Zero;

        private void GoTo(MovePoint destination)
        {
            MovePoint adjustedDestination = _maze.GetNearestPoint(destination, _currentPosition);
            if (adjustedDestination.X == -1 || adjustedDestination.Y == -1)
            {
                SetModelActivePosition(destination, false);
                return;
            }

            SetModelActivePosition(adjustedDestination, true);


            _currentRoad = _maze.FindRoad(_currentPosition, adjustedDestination);
            if (_currentRoad.Count == 0) return;

            //Stop current tween
            _tweenMove.Pause();
            DOTween.Kill(_tweenMove);

            _currentRoadId = _currentRoad.Count - 2;
            GoToNextTile();
        }

        private void OnDestroy()
        {
            if (!_isQuit)
            {
                UpdatePositionToServer();
            }
        }

        private bool _isQuit = false;

        private void OnApplicationQuit()
        {
            _isQuit = true;
            UpdatePositionToServer();
        }

        public void ResetInitMap()
        {
            _isInitPosition = false;
            _isInitItem = false;
            _lstMapItems.ForEach((mapItem) => DestroyImmediate(mapItem.gameObject));
        }

        private void UpdatePositionToServer()
        {
            if (_maze == null || _maze.Equals(null)) return;
            if (_maze.IsOnRoad(_currentPosition.X, _currentPosition.Y))
            {
                User.UpdateUserPosition(_currentPosition);
            }
        }

        private void GoToNextTile()
        {
            if (_currentRoadId >= 0)
            {
                MovePoint nextTilePosition = _currentRoad[_currentRoadId];
                UpdateAnimation(nextTilePosition);
                Move(nextTilePosition.X, nextTilePosition.Y);
                _currentRoadId--;
            }
            else
            {
                UpdatePositionToServer();
                UpdateStanding();
                DisplayModelPoint(false);
                if (_isOpenCampaign)
                {
                    _isOpenCampaign = false;
                    Menus.OpenCampaignMenu(_regionId.ToString());
                }
            }
        }

        private void UpdateStanding()
        {
            string animName = ModelCharacter.animationName;
            string nextAnimName = animName.Replace("walking", "standing");
            ModelCharacter.animationName = nextAnimName;
            ModelCharacter.animation.Play(ModelCharacter.animationName);
        }


        private void HandleAfterMove(int tileX, int tileY)
        {
            List<MapItemModel> lstItemModels = Redux.State.MapState.Items;
            if (Ultilities.IsEmpty(lstItemModels)) return;

            MapItemModel mapItemModel = lstItemModels.Find(x => x.X == tileX && x.Y == tileY);

            if (mapItemModel == null) return;
            Items.StepOnMapItemActionCreator(mapItemModel);

            MapItem mapItem = _lstMapItems.Find(x => x.X == tileX && x.Y == tileY);

            if (mapItem != null)
            {
                MapService.IsGotItem(tileX, tileY).Then((res) =>
                {
                    SoundService.Instance.PlaySound(SoundName.GetItem);

                    if (bool.Parse(res))
                    {
                        GainItemEffect goGainItemEffect =
                            Instantiate(_pfGainItemEffect, _goMap.transform).GetComponent<GainItemEffect>();
                        goGainItemEffect.transform.localPosition = mapItem.transform.localPosition;
                        goGainItemEffect.Init(mapItemModel.Quantity, mapItemModel.Type);
                        Destroy(mapItem.gameObject);
                        _lstMapItems.Remove(mapItem);
                    }
                });
            }
        }

        private void Move(int tileX, int tileY)
        {
            HandleAfterMove(tileX, tileY);

            float distance = Vector3.Distance(TiledMap.transform.localPosition,
                GetLocalPosition(tileX, tileY));
            float duration = 0.5f * distance / (2.0f / 3.0f) / 2.0f;
            _tweenMove = TiledMap.transform
                .DOLocalMove(GetLocalPosition(tileX, tileY), duration).OnComplete(() =>
                {
                    SetPosition(tileX, tileY);
                    GoToNextTile();
                });
        }

        private void UpdateAnimation(MovePoint nextTilePosition)
        {
            int difX = nextTilePosition.X - _currentPosition.X;
            int difY = nextTilePosition.Y - _currentPosition.Y;
            ModelCharacter.transform.localScale = Vector3.one;

            if (difX == -1 && difY == 0)
            {
                ModelCharacter.animationName = "side_walking";
            }
            else if (difX == 1 && difY == 0)
            {
                ModelCharacter.animationName = "side_walking";
                ModelCharacter.transform.localScale = new Vector3(-1, 1);
            }
            else if (difX == 0 && difY == -1)
            {
                ModelCharacter.animationName = "back_walking";
            }
            else if (difX == 0 && difY == 1)
            {
                ModelCharacter.animationName = "front_walking";
            }

            ModelCharacter.animation.Play(ModelCharacter.animationName);
        }


        private void UpdateCurrentPosition(int tileX, int tileY)
        {
            _currentPosition.X = tileX;
            _currentPosition.Y = tileY;
        }

        private void SetPosition(int tileX, int tileY)
        {
            UpdateCurrentPosition(tileX, tileY);
            TiledMap.transform.localPosition = GetLocalPosition(tileX, tileY);
        }

        Vector3 GetLocalPosition(float tileX, float tileY)
        {
            return new Vector3(tileX * -_tileWidthOnScreen, tileY * _tileHeightOnScreen);
        }


        MovePoint GetMovePoint(Vector3 touchPos)
        {
            int tileX = (int)(touchPos.x / _tileWidthOnScreen);
            int tileY = -(int)(touchPos.y / _tileHeightOnScreen);
            return MovePoint.Create(tileX, tileY);
        }

        private void SetModelActivePosition(MovePoint destination, bool isCorrect)
        {
            Vector3 pointPos = new Vector3((destination.X + 0.5f) * _tileWidthOnScreen,
                -(destination.Y + 0.3f) * _tileHeightOnScreen);
            ModelPoint.transform.localPosition = pointPos;

            string animationName = isCorrect ? "correct" : "uncorrect";

            ModelPoint.animation.Play(animationName, 1);
            ModelPoint.animationName = animationName;
        }

        private bool _isOpenCampaign;
        private int _regionId;

        void Update()
        {
            if (Menus.IsOpening()) return;
            if (!Input.GetMouseButtonDown(0) || EventSystem.current.currentSelectedGameObject != null) return;
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject(0)) return;

            Vector3 mousePos = Input.mousePosition;
            Vector3 wordInputPos = Camera.main.ScreenToWorldPoint(mousePos);


            RaycastHit2D[] allRaycast = Physics2D.RaycastAll(wordInputPos, Vector2.zero);

            if (allRaycast.Length >= 1)
            {
                RaycastHit2D gateHit = allRaycast.FirstOrDefault(x =>
                    x.transform != null && x.transform.name.Contains("MapCampaignGate"));

                RaycastHit2D tiledMapHit = allRaycast.FirstOrDefault(x =>
                    x.transform != null && x.transform.name.Contains("TiledMap"));


                if (gateHit.transform != null)
                {
                    MapCampaignGate mapCampaignGate = gateHit.transform.GetComponent<MapCampaignGate>();
                    MovePoint destination = MovePoint.Create((int)mapCampaignGate.X, (int)mapCampaignGate.Y);
                    MovePoint nearestPoint = _maze.GetNearestPoint(destination, _currentPosition);

                    if (nearestPoint.X == _currentPosition.X && nearestPoint.Y == _currentPosition.Y)
                    {
                        Menus.OpenCampaignMenu(mapCampaignGate.RegionId.ToString());
                    }
                    else
                    {
                        GoTo(nearestPoint);
                        _isOpenCampaign = true;
                        _regionId = mapCampaignGate.RegionId;
                    }
                }
                else
                {
                    _isOpenCampaign = false;
                    if (tiledMapHit.transform == null) return;

                    Vector3 touchPos = tiledMapHit.transform.InverseTransformPoint(tiledMapHit.point);
                    MovePoint destination = GetMovePoint(touchPos);
                    if (destination == null || !_maze.IsInMaze(destination)) return;

                    DisplayModelPoint(true);
                    GoTo(destination);
                }
            }
        }
    }
}