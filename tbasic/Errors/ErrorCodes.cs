// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
namespace Tbasic.Errors
{
    /// <summary>
    /// These codes indicate the at least partial success of a function
    /// </summary>
    public struct ErrorSuccess
    {
        /// <summary>
        /// The function is reporting no error
        /// </summary>
        public const int OK = 200;
        /// <summary>
        /// The function has created the requested data
        /// </summary>
        public const int Created = 201;
        /// <summary>
        /// The function believes the request was completed but cannot confirm it absolutely, 
        /// or the request is being processed and its success cannot be determined at the functions termination
        /// </summary>
        public const int Accepted = 202;
        /// <summary>
        /// The function is returning information from another source
        /// </summary>
        public const int NonAuthoritative = 203;
        /// <summary>
        /// The function completed successfully but has no content to return
        /// </summary>
        public const int NoContent = 204;
        /// <summary>
        /// The function completed with warnings, or its task was only partially completed
        /// </summary>
        public const int Warnings = 206;

        /// <summary>
        /// Determines whether the given code is a success code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsSuccess(int code)
        {
            return code >= 200 && code < 300;
        }
    }

    /// <summary>
    /// These codes represent errors because certain preconditions have not been met
    /// </summary>
    public struct ErrorClient
    {
        /// <summary>
        /// The function could not be completed because it was formatted incorrectly
        /// </summary>
        public const int BadRequest = 400;
        /// <summary>
        /// The function does not have permission to complete the command, but credentials may be supplied
        /// </summary>
        public const int Unauthorized = 401;
        /// <summary>
        /// The function has no way of executing with the current permissions
        /// </summary>
        public const int Forbidden = 403;
        /// <summary>
        /// The function could not locate requested data
        /// </summary>
        public const int NotFound = 404;
        /// <summary>
        /// There was a conflict accessing certain data, either too many instances of an object were requested or multiple objects were requested to occupy the same memory
        /// </summary>
        public const int Conflict = 409;
        /// <summary>
        /// The resource may currently be in use
        /// </summary>
        public const int Locked = 423;

        /// <summary>
        /// Determines whether the given code is an ErrorClient code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsError(int code)
        {
            return code >= 400 && code < 500;
        }
    }

    /// <summary>
    /// These codes represent errors because
    /// </summary>
    public struct ErrorServer
    {
        /// <summary>
        /// A generic exception occoured
        /// </summary>
        public const int GenericError = 500;
        /// <summary>
        /// The function has not been implemented
        /// </summary>
        public const int NotImplemented = 501;
        /// <summary>
        /// Data received through a gateway was invalid or poorly formatted
        /// </summary>
        public const int BadGateway = 502;
        /// <summary>
        /// There is no memory to perform the requested task
        /// </summary>
        public const int NoMemory = 507;

        /// <summary>
        /// Determines whether the given code is an ErrorServer code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsError(int code)
        {
            return code >= 500 && code < 600;
        }
    }
}
