using Jarvis.JsonStore.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jarvis.JsonStore.Core.Storage
{
    public interface IObjectStore
    {
        /// <summary>
        /// returns true if the object is new or updated, return 
        /// the stored object on the collection
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        Task<StoredObject> Store(TypeId type, Model.ApplicationId id, String jsonObject);

        /// <summary>
        /// return object stored using the application id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StoredObject> GetById(TypeId type, Model.ApplicationId id);

        /// <summary>
        /// Mark the object as removed.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StoredObject> DeleteById(TypeId type, Model.ApplicationId id);

        /// <summary>
        /// Return all data needed to de-duplicate the object
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<HashedData> GetHashedDataById(TypeId type, Model.ApplicationId id);
    }

}
