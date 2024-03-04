using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Architecture
{
	public static class IObservableExtension
	{
		public static IObservable<Unit> AsThunkObservable(this object observer)
		{
			var observable = observer as IObservable<Unit>;
			return observable;
		}
	}
}
