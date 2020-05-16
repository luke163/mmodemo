﻿using System.Threading;
using System.Threading.Tasks;

namespace FootStone.Client
{
    public interface IFSClient
    {
        /// <summary>
        /// 启动fs 
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the work performed.</returns>
        void Start(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 停止fs
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the work performed.</returns>
        /// <remarks>
        /// A stopped silo cannot be restarted.
        /// If the provided <paramref name="cancellationToken"/> is canceled or becomes canceled during execution, the silo will terminate ungracefully.
        /// </remarks>
        void Stop(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 创建Session
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<IFSSession> CreateSession(string ip, int port);

        /// <summary>
        /// 主线程调用
        /// </summary>
        void Update();
     
    }
}
