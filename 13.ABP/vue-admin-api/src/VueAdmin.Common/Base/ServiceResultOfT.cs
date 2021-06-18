using VueAdmin.Common.Base.Enum;

namespace VueAdmin.Common.Base
{
    /// <summary>
    /// 服务层响应实体(泛型)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResult<T> : ServiceResult where T : class
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 响应成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public void IsSuccess(T data = null, string message = "")
        {
            Message = message;
            Code = ServiceResultCode.Succeed;
            Data = data;
        }
    }
}
