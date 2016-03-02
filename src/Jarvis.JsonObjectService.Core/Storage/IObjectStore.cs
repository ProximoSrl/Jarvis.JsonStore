using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.JsonObjectService.Core.Storage
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

    }

}
