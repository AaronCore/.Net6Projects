using System;

namespace EFCoreWebAPI.Sample.DtoModels
{
    /// <summary>
    /// 档案Dto
    /// </summary>
    public class ArchiveDto : StudentDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid ArchiveId { get; set; }
        /// <summary>
        /// 档案编号
        /// </summary>
        public string ArchiveNo { get; set; }
    }
}
