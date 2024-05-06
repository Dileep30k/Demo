using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repository
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
