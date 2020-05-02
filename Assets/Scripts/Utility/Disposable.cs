using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8#examples
    // https://clickan.click/idisposable/
    public class Disposable : System.IDisposable
    {
        private bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);

            // GC によるデストラクタ呼び出しを抑制する
            System.GC.SuppressFinalize(this);
        }

        // ユーザーコードから呼ばれた場合は isDisposing = true となり、マネージド/アンマネージドリソースを解放できる。
        // isDisposing = false の場合はファイナライザによって呼び出されており、他オブジェクトへの参照は有効でないため、アンマネージドリソースのみ解放できる。
        protected virtual void Dispose(bool isDisposing)
        {
            if (!this.m_disposed)
            {
                if (isDisposing)
                {
                    // dispose managed resources
                }

                // dispose unmanaged resources

                m_disposed = true;
            }
        }

        ~Disposable()
        {
            // Dispose() を呼び忘れても、GC によって Dispose() が呼ばれるようにする
            Dispose(false);
        }
    }
}
