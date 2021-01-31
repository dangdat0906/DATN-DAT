using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace APP.UTILS
{
    public enum StatusEnum
    {
        [Description("Tất cả trạng thái")]
        All = -1,
        [Description("Sử dụng")]
        Active = 1,
        [Description("Không sử dụng")]
        Unactive = 0,
        [Description("Ẩn")]
        Removed = 2
    }
    public enum RolesEnum
    {
        [Description("Thêm mới")]
        Create,
        [Description("Sửa")]
        Update,
        [Description("Xóa")]
        Delete,
        [Description("Phê duyệt")]
        Approval


    }
}
public enum ContentTypeEnum
{
    [Description("Bài viết thường")]
    Normal = 1,
    [Description("Bài viết ảnh")]
    Image = 2,
    [Description("Bài viết video")]
    Video = 3,
}
    //Enum Status cho Contents
    public enum ContentStatusEnum
    {
        [Description("Tất cả")]
        All = -1,
        [Description("Tạo")]
        Created = 1,
        [Description("Chờ phê duyệt")]
        Approving = 2,
        [Description("Đã phê duyệt")]
        Approved = 3,
        [Description("Đăng tải")]
        Published = 4,
        [Description("Thu hồi")]
        Revoked = 5,
        [Description("Hủy")]
        Rejected = 6,
        [Description("Bị gỡ")]
        Removed = 7,
    }
    //Enum Status cho ContentType

public enum ConfigTypeEnum
{
    [Description("List")]
    List = 1,
    [Description("Slide")]
    Slide = 2
}
public enum ConfigDisplayTypeEnum
{
    [Description("Ngang")]
    Horizontal = 1,
    [Description("Dọc")]
    Vertical = 2
}
public enum ConfigPositionEnum
{
    [Description("Trái")]
    Left = 1,
    [Description("Phải")]
    Right = 2

}



