using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningFucker.Handler
{
    public interface ITaskHandler
    {
        /// <summary>
        /// 开始处理任务
        /// </summary>
        /// <param name="fucker"></param>
        bool Start(Fucker fucker);

        bool Stop();

        TaskStatus TaskStatus { get; }

        Action<object, TaskStatus> StatusChanged { get; set; }

    }
}
