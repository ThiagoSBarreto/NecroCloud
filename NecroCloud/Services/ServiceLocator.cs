using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ServiceLocator : IServiceLocator
    {
        #region [ Atributos ]
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> _singleTons;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> _services;

        /// <summary>
        /// 
        /// </summary>
        private static ServiceLocator _instance = new ServiceLocator();

        /// <summary>
        /// 
        /// </summary>
        public static ServiceLocator Current { get { return _instance; } }
        #endregion
        #region [ Construtor ]
        /// <summary>
        /// 
        /// </summary>
        private ServiceLocator()
        {
            _singleTons = new Dictionary<string, object>();
            _services = new Dictionary<string, object>();
        }
        #endregion

        #region [ Metodos ]
        public void RegisterSingleton<T, T2>() where T2 : class
        {
            var check = typeof(T2).GetInterface(typeof(T).Name);
            if (check == null)
            {
                throw new ApplicationException($"O Singleton \"{typeof(T2).Name}\" deve implementar a interface \"{typeof(T).Name}\" para poder ser registrado.");
            }
            if (_singleTons.Keys.Contains(typeof(T).Name))
            {
                throw new ApplicationException($"Duplicated Singleton Register \"{typeof(T).Name}\"");
            }
            _singleTons.Add(typeof(T).Name, Activator.CreateInstance(typeof(T2)));
        }

        public T GetSingleton<T>()
        {
            try
            {
                return (T)_singleTons[typeof(T).Name];
            }
            catch
            {
                throw new ApplicationException($"Singleton Not Found \"{typeof(T).Name}\"");
            }
        }

        public void RegisterService<T>() where T : class
        {
            var instance = Activator.CreateInstance(typeof(T));
            string name = typeof(T).Name;
            if (_services.Keys.Contains(name))
            {
                throw new ApplicationException($"Duplicated Sevices Registred \"{name}\"");
            }

            var check = typeof(T).GetInterface(typeof(IServiceBase).Name);
            if (check == null)
            {
                throw new ApplicationException($"Services Must Implement \"IServiceBase\" to be Registered!");
            }
            _services.Add(name, instance);

            try
            {
                (instance as IServiceBase).Configure();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Configure Service: {name}. See InnerException for more details.", ex);
            }

            try
            {
                (instance as IServiceBase).Start();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Start Service: {name}. See InnerException for more details.", ex);
            }
        }

        public T GetService<T>() where T : class
        {
            string name = typeof(T).Name;
            if (!_services.Keys.Contains(name))
            {
                throw new ApplicationException($"Service Not Found \"{name}\"");
            }
            return (T)_services[name];
        }

        public void UnRegisterService<T>() where T : class
        {
            string name = typeof(T).Name;
            if (_services.Keys.Contains(name))
            {
                (_services[name] as IServiceBase).Stop();
                (_services[name] as IServiceBase).Dispose();
                _services.Remove(name);
            }
        }
        #endregion
    }
}
