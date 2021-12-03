using Microsoft.EntityFrameworkCore.Migrations;

namespace Upico.Migrations
{
    public partial class SeedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'8a3d8bd2-aa89-44d6-b458-836b2f30ea70', N'User', N'USER', N'3f77d147-a705-4cb1-929e-95f7a18cbf1e')
                INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'ce25ea68-7b7e-4388-bad5-ebf297d7e65c', N'Admin', N'ADMIN', N'f0bccabe-9e70-459f-a273-13bb1160aeda')
            ");


            migrationBuilder.Sql(@"
                INSERT [dbo].[AspNetUsers] ([Id], [DisplayName], [Bio], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'1320abf3-35fa-46f9-b261-4385c065a1ba', N'Nghia', NULL, N'nghiax', N'NGHIAX', N'nghia@test.com', N'NGHIA@TEST.COM', 0, N'AQAAAAEAACcQAAAAENE2b8rI5/dca8RcRTH6+eiocT29VRIRM98tBbGPQrJmMe3nFWo+bt9c6C9NCEpn3A==', N'IYUMIPDFXEHOOAY4AYGBR5SC7DUOJQZG', N'97327e98-ea0c-4531-83a1-9e225618b60b', NULL, 0, 0, NULL, 1, 0)
                INSERT [dbo].[AspNetUsers] ([Id], [DisplayName], [Bio], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'2da21c12-5118-459e-b671-ea57dd21b764', N'Admin', NULL, N'admin', N'ADMIN', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEDY23KkAuHJr9ycdDdQfq2X0fw/ZwejnLdTbVfPXT3eKB4SDePe5rciJg9rll0f7hA==', N'6UDFJUKFSZ3OYLU7E7R3F5SNX7LATD7N', N'b67c6793-fe87-4d58-9b54-6542a91d4ae3', NULL, 0, 0, NULL, 1, 0)
                INSERT [dbo].[AspNetUsers] ([Id], [DisplayName], [Bio], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'461b35ae-f7b9-446f-a9ee-9f46cc88df4d', N'HuyBoDo', NULL, N'huy', N'HUY', N'huy@test.com', N'HUY@TEST.COM', 0, N'AQAAAAEAACcQAAAAEPxzPTBQGgreblGK4At3z8lu9MfP14CaePlB2DgnPb9z27R+F/dEtBCbva9Rb834yQ==', N'P5V65BWQZVQ2XULUQGB7WUE3NO5YCIJX', N'35d374a6-fe2c-45d0-970a-cf2fbedf28ca', NULL, 0, 0, NULL, 1, 0)
                INSERT [dbo].[AspNetUsers] ([Id], [DisplayName], [Bio], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'5a5b6ef1-2638-4032-865c-38d13157f613', N'Vinh', NULL, N'vinh', N'VINH', N'vinh@gmail.com', N'VINH@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEI67q5u9y+dYQENPB+L7pZoLWQNXujNL3/sr7mXvdr8Imzfho4Rp0V/Zw1OWRi2XZQ==', N'NM6T5NJR5KU6SKGAJKQMEUF67UPCKISU', N'0a01159a-c8c9-4e7c-8ede-08c79cdc7bc5', NULL, 0, 0, NULL, 1, 0)
                INSERT [dbo].[AspNetUsers] ([Id], [DisplayName], [Bio], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'6ea1478b-e016-4f3c-9717-380cbe3b1e06', N'Tam', NULL, N'tam', N'TAM', N'tiger@test.com', N'TIGER@TEST.COM', 0, N'AQAAAAEAACcQAAAAEPLLFYck/o+lgFmHFNT8JSOErgwD5oiHIzIP5h+pKzwIUnANHk/cibbKAaNr2rTNKQ==', N'ZNDJZ5SHHP3QFDSIDKO3RRC33G2ELHZ4', N'318ddcb5-ef35-4e4d-819c-5982f329ebe2', NULL, 0, 0, NULL, 1, 0)
            ");

            migrationBuilder.Sql(@"
                INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'1320abf3-35fa-46f9-b261-4385c065a1ba', N'8a3d8bd2-aa89-44d6-b458-836b2f30ea70')
                INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'461b35ae-f7b9-446f-a9ee-9f46cc88df4d', N'8a3d8bd2-aa89-44d6-b458-836b2f30ea70')
                INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'5a5b6ef1-2638-4032-865c-38d13157f613', N'8a3d8bd2-aa89-44d6-b458-836b2f30ea70')
                INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'6ea1478b-e016-4f3c-9717-380cbe3b1e06', N'8a3d8bd2-aa89-44d6-b458-836b2f30ea70')
                INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'2da21c12-5118-459e-b671-ea57dd21b764', N'ce25ea68-7b7e-4388-bad5-ebf297d7e65c')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
