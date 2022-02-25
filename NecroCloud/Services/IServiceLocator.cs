using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    /// <summary>
    /// Interface para registro de Singletons e Serviços
    /// </summary>
    internal interface IServiceLocator
    {
        /// <summary>
        /// Registra uma instancia unica que será reutilizada por toda a aplicação
        /// </summary>
        /// <typeparam name="Interface">Interface que a Classe Implementa</typeparam>
        /// <typeparam name="Class">Classe a ser salva como Singleton</typeparam>
        void RegisterSingleton<Interface, Class>() where Class : class;

        /// <summary>
        /// Recupera a Instancia salva do Singleton
        /// </summary>
        /// <typeparam name="T">Instancia do Singleton</typeparam>
        /// <returns></returns>
        T GetSingleton<T>();

        /// <summary>
        /// Registra um tipo para ser instanciado quando solicitado
        /// </summary>
        /// <typeparam name="Interface">Interface que a Classe implementa</typeparam>
        /// <typeparam name="Class">Classe a ser salva</typeparam>
        void RegisterService<Class>() where Class : class;

        void UnRegisterService<Class>() where Class : class;
    }
}
