using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    /// <summary>
    /// Interface utilizada pelo Logger, contendo as funções e auxliares para geração de LOGS
    /// </summary>
    internal interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="maxLogSize"></param>
        /// <param name="maxLogSizeType"></param>
        /// <param name="logPersistenceTime"></param>
        /// <param name="logPersistenceTimeType"></param>
        void Configurar(string path = "", int maxLogSize = 30, TipoTamanhoLog maxLogSizeType = TipoTamanhoLog.MB, int logPersistenceTime = 3, TipoPersistencia logPersistenceTimeType = TipoPersistencia.DIAS);

        /// <summary>
        /// Cria uma mensagem de LOG e grava em Arquivo
        /// </summary>
        /// <param name="ex">Exception Gerada pelo Sistema</param>
        /// <param name="logType">Tipo do LOG</param>
        /// /// <param name="message">Mensagem Opcional sobre o erro</param>
        void CreateLog(string message, LogType logType = LogType.MESSAGE, Exception ex = null);
    }

    /// <summary>
    /// Tipos de LOGS que podem ser gerados
    /// </summary>
    internal enum LogType
    {
        /// <summary>
        /// Log de Nivel DEBUG ( não é gerado em modo release )
        /// </summary>
        DEBUG,
        /// <summary>
        /// Log de Nivel WARNING ( Atenção )
        /// </summary>
        WARNING,
        /// <summary>
        /// Log de Nivel ERRO ( erro no metodo/chamada/tipo/conversão )
        /// </summary>
        ERROR,
        /// <summary>
        /// Log de Nivel CRITICAL ( erro do Sistema/Instabilidade )
        /// </summary>
        CRITICAL,
        /// <summary>
        /// Log de Nivel MESSAGE ( mensagem de inicio/fim de Metodo/Atividade )
        /// </summary>
        MESSAGE,
        /// <summary>
        /// Log de Nivel CONSOLE ( exibida apenas no Console da Aplicacao )
        /// </summary>
        CONSOLE
    }

    /// <summary>
    /// Unidade de tamanho do Arquivo de LOG
    /// </summary>
    internal enum TipoTamanhoLog
    {
        /// <summary>
        /// O tamanho maximo do LOG será: TamanhoMaximoLog * 1
        /// </summary>
        KB = 1,
        /// <summary>
        /// O tamanho maximo do LOG será: TamanhoMaximoLog * 1024
        /// </summary>
        MB = 1024,
        /// <summary>
        /// O tamanho maximo do LOG será: TamanhoMaximoLog * 1048576
        /// </summary>
        GB = 1048576,
        /// <summary>
        /// O tamanho maximo do LOG será: TamanhoMaximoLog * 1073741824
        /// </summary>
        TB = 1073741824
    }

    /// <summary>
    /// Unidade de tempo da persistencia dos arquivos
    /// </summary>
    internal enum TipoPersistencia
    {
        /// <summary>
        /// TempoPersistenciaLog será em HORAS
        /// </summary>
        HORAS,
        /// <summary>
        /// TempoPersistenciaLog será em DIAS
        /// </summary>
        DIAS,
        /// <summary>
        /// TempoPersistenciaLog será em MESES
        /// </summary>
        MESES,
        /// <summary>
        /// TempoPersistenciaLog será em ANOS
        /// </summary>
        ANOS
    }
}
