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
        /// Collection respossable for storing all registered SingleTons
        /// </summary>
        private Dictionary<string, object> _singleTons;

        /// <summary>
        /// Collection responsable for storing all registered Services
        /// </summary>
        private Dictionary<string, object> _services;

        /// <summary>
        /// Self creating singleton of this instance of ServiceLocator
        /// </summary>
        private static ServiceLocator _instance = new ServiceLocator();

        /// <summary>
        /// Object for recovering the current instance of ServiceLocator
        /// </summary>
        public static ServiceLocator Current { get { return _instance; } }
        #endregion
        #region [ Construtor ]
        /// <summary>
        /// Service Locator constructor
        /// </summary>
        private ServiceLocator()
        {
            _singleTons = new Dictionary<string, object>();
            _services = new Dictionary<string, object>();
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Register a singleton into ServiceLocator, only one instance can existe at time
        /// </summary>
        /// <typeparam name="T">Interface that must be implemented by the SingleTon</typeparam>
        /// <typeparam name="T2">Class that implements the T interface</typeparam>
        /// <exception cref="ApplicationException">Throw ApplicationExeption in case of the Class not implementing the interface or if the SingleTon Allready exists</exception>
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

        /// <summary>
        /// Gets a Singleton by the given interface
        /// </summary>
        /// <typeparam name="T">SingleTon Interface</typeparam>
        /// <returns>The instance of the Singleton</returns>
        /// <exception cref="ApplicationException">Throw ApplicationExeption in case of the Singleton was not found</exception>
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

        /// <summary>
        /// Register a service that will run until it's Stoped or Disposed
        /// Automatically calls Configure and Start at registration
        /// </summary>
        /// <typeparam name="T">The Service class</typeparam>
        /// <exception cref="ApplicationException">Throw ApplicationException in case of Service Allready registered or when it doesn't implement IServiceBase</exception>
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

        /// <summary>
        /// Get a service by it's class
        /// </summary>
        /// <typeparam name="T">The service class</typeparam>
        /// <returns>The instanci of given service</returns>
        /// <exception cref="ApplicationException"></exception>
        public T GetService<T>() where T : class
        {
            string name = typeof(T).Name;
            if (!_services.Keys.Contains(name))
            {
                throw new ApplicationException($"Service Not Found \"{name}\"");
            }
            return (T)_services[name];
        }

        /// <summary>
        /// Unregister the service by it's name class, calling the methods Stop and Dispose
        /// </summary>
        /// <typeparam name="T">The Service class</typeparam>
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
