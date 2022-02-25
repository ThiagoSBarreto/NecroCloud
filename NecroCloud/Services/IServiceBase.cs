using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    public interface IServiceBase
    {
        /// <summary>
        /// Metodo que será chamado ao carregar o serviço
        /// </summary>
        bool Configure();
        /// <summary>
        /// Metodo que será chamado apos configuração
        /// </summary>
        bool Start();
        /// <summary>
        /// Metodo que será chamado para Parar o serviço, mas a instancia continuará ativa
        /// sendo necessario chamar "Start" novamente
        /// </summary>
        bool Stop();
        /// <summary>
        /// Metodo que será chamado ao descarregar o serviço
        /// </summary>
        bool Dispose();
    }
}
