using System;
using System.Threading;
namespace Tpl.Samples
{
    public interface IStrategy
    {
        int Sum(Node node);
    }
}
