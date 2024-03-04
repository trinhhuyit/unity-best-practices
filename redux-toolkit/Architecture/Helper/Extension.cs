using System;
using RSG;
using UniRx;

namespace Architecture
{
    public static class Extension
    {
        public static Promise<T> ToPromise<T>(this IObservable<T> observer)
        {
            Promise<T> promise = new Promise<T>((resolve, reject) => { observer.CatchIgnore(reject).Subscribe(resolve); });
            return promise;
        }
    }
}