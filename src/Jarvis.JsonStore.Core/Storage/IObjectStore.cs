﻿using System;
using System.Collections;
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
        Task<StoredObject> Store(String type, String id, String jsonObject);

        /// <summary>
        /// return object stored 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StoredObject> GetById(String type, String id);

        /// <summary>
        /// Mark the object as removed.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StoredObject> DeleteById(String type, String id);

    }

}
