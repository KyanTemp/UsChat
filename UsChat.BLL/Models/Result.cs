using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.BLL.Models
{
    //{
    //Generic type parameters are replaced with the type arguments during compilation.
    //maximize code reuse, type safety, and performance.
    //} source: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics
    //
    //{
    //In a generic type or method definition, a type parameter is a placeholder for a specific type that a client specifies when they create an instance of the generic type
    //} source: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-type-parameters
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public T Data { get; }
        private Result(T data, bool isSuccess, string errorMessage)
        {
            Data = data;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
        public static Result<T> Success(T data) => new Result<T>(data, true, null);
        public static Result<T> Failure(string errorMessage) => new Result<T>(default, false, errorMessage);
    }
    public class Result
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        private Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
        public static Result Success() => new Result(true, null);
        public static Result Failure(string errorMessage) => new Result(false, errorMessage);
    }

}
