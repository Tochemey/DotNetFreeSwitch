using System.Threading.Tasks;

namespace ModFreeSwitch.Common {

        /// <summary>
        ///     Defines an asynchronous event handler such that events may be executed in an asynchronous fashion
        /// </summary>
        public delegate Task AsyncEventHandler<in T>(object source, T e);
    
}