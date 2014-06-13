using System;
using System.Threading;

namespace LzEngine.Util
{
    public static class LockExtension
    {
        public static void DoReadLock(this ReaderWriterLockSlim locker,
                                      Action action)
        {
            locker.EnterReadLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static T DoReadLock<T>(this ReaderWriterLockSlim locker,
                                      Func<T> functor)
        {
            locker.EnterReadLock();
            T returnValue;
            try
            {
                returnValue = functor();
            }
            finally
            {
                locker.ExitReadLock();
            }
            return returnValue;
        }

        public static void DoWriteLock(this ReaderWriterLockSlim locker,
                                       Action action)
        {
            locker.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static T DoWriteLock<T>(this ReaderWriterLockSlim locker,
                                       Func<T> functor)
        {
            locker.EnterWriteLock();
            T returnValue;
            try
            {
                returnValue = functor();
            }
            finally
            {
                locker.ExitWriteLock();
            }
            return returnValue;
        }
    }
}