Game's Concept
- This is a real-time multiplayer board game
- Each player will control a Chess to move (1 time) and attack (1 time) in a turn
- The player will need to defeat all opponent's monsters 

Structures
- Write utilities for Unidux (a Redux framework for Unity) for structuring the most straightforward way for handling data that got from POST/PUT/GET APIs in Game for UIs (redux-toolkit folder)
    - Loading data based on fulfilled/pending/rejected actions
    - Middleware for authenticating/global error catching/logging and another thing for Redux

- Use the Commands Design Pattern for handling Monster's skills combined with many things like effects (dragonbones effect, DOTWeen Effect) in and out of animation. 
    - One of the most complicated things in this project is that many monsters have different skills, and the number of monsters will increase.           
    - Some actions (live move, death) in a skill can be reused for many monsters. 
    => Use a Command Design Pattern  (clear input with clear output) combined with an ActionQueue (for receiving and ensuring synchronous handling for all packages from WebSocket). We can provide the scalability of the Monster's system (commands folder). 

- Use the Breadth First Search Algorithm to find the shortest way on the maze so the player can move to the touched point on the map to find items on the map. This approach is also used for AI on the game server (simple-ai-for-board-game folder)