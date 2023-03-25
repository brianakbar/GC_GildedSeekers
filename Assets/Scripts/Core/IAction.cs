using System;
using System.Collections.Generic;

namespace Creazen.Seeker.Core {
    public interface IAction {
        IEnumerable<Type> ExcludeType();
        void Cancel();
    }
}