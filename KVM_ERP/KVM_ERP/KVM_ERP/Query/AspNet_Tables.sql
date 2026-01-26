USE [SYNC]
GO
/****** Object:  Table [dbo].[ApplicationRoleGroups]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationRoleGroups](
	[RoleId] [nvarchar](128) NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ApplicationRoleGroups] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationUserGroups]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationUserGroups](
	[UserId] [nvarchar](128) NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ApplicationUserGroups] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
	[RMenuType] [nvarchar](128) NULL,
	[RControllerName] [nvarchar](128) NULL,
	[RMenuGroupId] [smallint] NULL,
	[RMenuGroupOrder] [smallint] NULL,
	[RMenuIndex] [nvarchar](128) NULL,
	[SDPTID] [int] NOT NULL,
	[RImageClassName] [varchar](1050) NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[User_Id] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[UserId] [nvarchar](128) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
	[NPassword] [nvarchar](50) NULL,
	[BrnchId] [int] NOT NULL,
	[UBrnchName] [nvarchar](100) NULL,
	[DBrnchId] [int] NOT NULL,
	[DeptName] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MenuRoleMaster]    Script Date: 17/08/2023 12:10:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuRoleMaster](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[LinkText] [varchar](100) NOT NULL,
	[ActionName] [varchar](100) NOT NULL,
	[ControllerName] [varchar](100) NOT NULL,
	[Roles] [varchar](250) NOT NULL,
	[MenuGId] [smallint] NULL,
	[MenuGIndex] [smallint] NULL,
	[ImageClassName] [varchar](100) NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'003052bc-baef-4034-909e-0fe93316cd3b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'003052bc-baef-4034-909e-0fe93316cd3b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'008e4ba4-4491-4a60-92ff-837c92d5a533', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'008e4ba4-4491-4a60-92ff-837c92d5a533', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'009823bc-1fab-4a4c-a791-d86cf8b3b5e8', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'009823bc-1fab-4a4c-a791-d86cf8b3b5e8', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'04db2450-b80a-40bd-bef1-19629c053c49', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'04db2450-b80a-40bd-bef1-19629c053c49', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'04db2450-b80a-40bd-bef1-19629c053c49', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'05e00c77-9221-4b2b-a5e5-2bbd2ac6ca7c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'05e00c77-9221-4b2b-a5e5-2bbd2ac6ca7c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'089f90cc-ee43-41ae-b6a5-bc3f687d96f5', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'089f90cc-ee43-41ae-b6a5-bc3f687d96f5', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'0b6a7382-f16f-42a0-8709-e6e5479bad3f', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'0b6a7382-f16f-42a0-8709-e6e5479bad3f', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'0b8feab9-e7b0-4911-9c69-4fa7b293146f', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'0b8feab9-e7b0-4911-9c69-4fa7b293146f', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'0b8feab9-e7b0-4911-9c69-4fa7b293146f', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1817aa37-97cf-4a2e-8f3d-252f6d70737c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1817aa37-97cf-4a2e-8f3d-252f6d70737c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1817aa37-97cf-4a2e-8f3d-252f6d70737c', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1b3a1693-a94d-48c0-95e7-83cdb27600df', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1b3a1693-a94d-48c0-95e7-83cdb27600df', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1c0e1406-2911-4faa-a069-9e68656b439e', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1c0e1406-2911-4faa-a069-9e68656b439e', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1e262a0a-eecf-4f5e-ae48-275a21443c41', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1e262a0a-eecf-4f5e-ae48-275a21443c41', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1e262a0a-eecf-4f5e-ae48-275a21443c41', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1e262a0a-eecf-4f5e-ae48-275a21443c41', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1f190114-62bf-4659-b560-35726eba9bd5', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'1f190114-62bf-4659-b560-35726eba9bd5', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'224719a8-1dda-46b2-b4a5-061df4fbbff6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'224719a8-1dda-46b2-b4a5-061df4fbbff6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'227ade1c-d9e3-473f-8949-c77264bbba45', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'227ade1c-d9e3-473f-8949-c77264bbba45', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'252c5ccf-1cca-4128-9fc6-28a2300e263a', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'252c5ccf-1cca-4128-9fc6-28a2300e263a', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'268bf640-c4f5-4689-b5e0-a182d3ae1791', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'268bf640-c4f5-4689-b5e0-a182d3ae1791', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'273fe8d9-3431-43da-b7e2-b3aa5dae5375', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'273fe8d9-3431-43da-b7e2-b3aa5dae5375', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'273fe8d9-3431-43da-b7e2-b3aa5dae5375', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 3)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 4)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27ba93d5-b6c6-4e5d-9129-e88e7478b39e', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'27ba93d5-b6c6-4e5d-9129-e88e7478b39e', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'295cc85f-cb67-4207-bd5c-aff996c50635', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'295cc85f-cb67-4207-bd5c-aff996c50635', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'29d4bd40-1a56-47da-a10b-5ee676cba147', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'29d4bd40-1a56-47da-a10b-5ee676cba147', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2aa40266-657d-46b8-88bf-9d87ca5cdb9f', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2aa40266-657d-46b8-88bf-9d87ca5cdb9f', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2b6bdf46-3ac2-4cc0-bda8-e0ec5be01bd4', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2b6bdf46-3ac2-4cc0-bda8-e0ec5be01bd4', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2c99a770-e7d4-45a5-ac84-b36bab74ecda', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2c99a770-e7d4-45a5-ac84-b36bab74ecda', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2c99a770-e7d4-45a5-ac84-b36bab74ecda', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2dd329dd-75bc-43b7-baba-5edde4db5f78', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2dd329dd-75bc-43b7-baba-5edde4db5f78', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2ea983bc-2514-43b3-82a9-268ef1d654ed', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2ea983bc-2514-43b3-82a9-268ef1d654ed', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2eed8755-5ad9-430f-a270-020e89c2245c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2eed8755-5ad9-430f-a270-020e89c2245c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2f80ac4b-354c-43ee-866e-daca175146b7', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'2f80ac4b-354c-43ee-866e-daca175146b7', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3154d802-3802-4f6b-826b-5b153f50aadd', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3154d802-3802-4f6b-826b-5b153f50aadd', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'346e8175-8527-440c-85d6-d4f62039a609', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'346e8175-8527-440c-85d6-d4f62039a609', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'346e8175-8527-440c-85d6-d4f62039a609', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'349d82b5-2e2e-4425-a355-4a222a4f6e2e', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'349d82b5-2e2e-4425-a355-4a222a4f6e2e', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3929e6c2-650c-4ecd-9621-02636fecf01d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3929e6c2-650c-4ecd-9621-02636fecf01d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3b987631-fceb-4a44-a292-bcba10f23503', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3b987631-fceb-4a44-a292-bcba10f23503', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3c605d07-96ce-49a1-b43c-7b9b4646f6b7', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3c605d07-96ce-49a1-b43c-7b9b4646f6b7', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3e1ff3c9-1813-4836-9fa6-da61161cc9df', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'3e1ff3c9-1813-4836-9fa6-da61161cc9df', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'41050157-8f6b-413e-a389-9cab38766b89', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'41050157-8f6b-413e-a389-9cab38766b89', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'41ebd7fc-3372-41b9-a4cf-7db380328621', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'41ebd7fc-3372-41b9-a4cf-7db380328621', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4304038b-dcd5-45fa-a752-fed8e74990e6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4304038b-dcd5-45fa-a752-fed8e74990e6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'434e4bef-8678-4ef3-9819-cf16dc0538a2', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'434e4bef-8678-4ef3-9819-cf16dc0538a2', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'45477ed9-faf0-4ecd-806e-12a66e5b182c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'45477ed9-faf0-4ecd-806e-12a66e5b182c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'45477ed9-faf0-4ecd-806e-12a66e5b182c', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'46a33275-57a3-41ea-b43c-1df73d5cbba0', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'46a33275-57a3-41ea-b43c-1df73d5cbba0', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'47665c5a-21d9-4194-b079-92e913af2683', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'47665c5a-21d9-4194-b079-92e913af2683', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'47895b55-cd23-4b8c-96ce-7a8c721c7501', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'47895b55-cd23-4b8c-96ce-7a8c721c7501', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'48360af1-14ca-444b-b6bd-8fecf889e29f', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'48360af1-14ca-444b-b6bd-8fecf889e29f', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4a51ee63-c8fe-41f3-bd26-d19eeac8e78d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4a51ee63-c8fe-41f3-bd26-d19eeac8e78d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4f8122ae-2f21-4419-bee5-78e2e86c8fec', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'4f8122ae-2f21-4419-bee5-78e2e86c8fec', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'52ae9049-716b-40ac-bdd8-595b2ff72818', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'52ae9049-716b-40ac-bdd8-595b2ff72818', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'55b5a43a-b21e-45c0-9112-1c9ffb361ffd', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'55b5a43a-b21e-45c0-9112-1c9ffb361ffd', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'58fc4203-c222-4695-8aa1-e3009578c091', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'58fc4203-c222-4695-8aa1-e3009578c091', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'5c5aec04-88da-4b84-80f6-83f278a3c316', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'5c5aec04-88da-4b84-80f6-83f278a3c316', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'5ff43aa9-4375-442b-8dfc-aad5e98e711e', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'5ff43aa9-4375-442b-8dfc-aad5e98e711e', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'62761c93-fb91-4cca-a40d-096cd4e8e414', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'62761c93-fb91-4cca-a40d-096cd4e8e414', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'664c641a-b623-4b78-8859-ab93e708dcec', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'664c641a-b623-4b78-8859-ab93e708dcec', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'66bce6fe-4480-4b3b-b291-27f0a6adc189', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'66bce6fe-4480-4b3b-b291-27f0a6adc189', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68702ee4-9ba1-484a-b7da-bbe63ff92534', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68702ee4-9ba1-484a-b7da-bbe63ff92534', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68702ee4-9ba1-484a-b7da-bbe63ff92534', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68b75beb-898d-4287-8038-629da9609c84', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68b75beb-898d-4287-8038-629da9609c84', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68c48e9a-735d-44b1-8a86-f174d9e188e7', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'68c48e9a-735d-44b1-8a86-f174d9e188e7', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'6b63e9f5-3988-4bd8-a00a-46ad924835d6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'6b63e9f5-3988-4bd8-a00a-46ad924835d6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'73a06379-5875-4ff1-a68a-db11d095cd51', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'73a06379-5875-4ff1-a68a-db11d095cd51', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'73a06379-5875-4ff1-a68a-db11d095cd51', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'73a06379-5875-4ff1-a68a-db11d095cd51', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'7487606c-b996-4157-8632-2e0b7d76ce29', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'7487606c-b996-4157-8632-2e0b7d76ce29', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'75facf1c-374d-48c9-8141-d6913051deb0', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'75facf1c-374d-48c9-8141-d6913051deb0', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'760dd5f6-dd3f-4db9-a7a8-c5279921e5ec', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'760dd5f6-dd3f-4db9-a7a8-c5279921e5ec', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'76635a55-43b8-4e39-bf0a-ef56076680c6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'76635a55-43b8-4e39-bf0a-ef56076680c6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'778323a9-a367-4da0-aaeb-444ff55bfd4b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'778323a9-a367-4da0-aaeb-444ff55bfd4b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'8198891b-6206-42c6-8ae1-afeba734946b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'8198891b-6206-42c6-8ae1-afeba734946b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'83f11971-94d7-4d00-aaf6-362ac489ad60', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'83f11971-94d7-4d00-aaf6-362ac489ad60', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'893342df-99f4-422d-804a-1e1f98700547', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'893342df-99f4-422d-804a-1e1f98700547', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'893342df-99f4-422d-804a-1e1f98700547', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'8ada39fb-92f0-41be-a0b3-1b4aed28990b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'8ada39fb-92f0-41be-a0b3-1b4aed28990b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'8ada39fb-92f0-41be-a0b3-1b4aed28990b', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9171a4b8-5349-4b48-93a6-12bacf724009', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9171a4b8-5349-4b48-93a6-12bacf724009', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9171a4b8-5349-4b48-93a6-12bacf724009', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9226ef62-6122-4c86-9e7e-7765ee64bdb6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9226ef62-6122-4c86-9e7e-7765ee64bdb6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9bbdc56c-4994-49c3-960b-9e4b59e6fc2d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9bbdc56c-4994-49c3-960b-9e4b59e6fc2d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9bc02953-e816-490e-aa24-907871b5bd89', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9bc02953-e816-490e-aa24-907871b5bd89', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9bc02953-e816-490e-aa24-907871b5bd89', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c17d10e-25d0-4dab-b28a-b3b11972a33d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c17d10e-25d0-4dab-b28a-b3b11972a33d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c201a6a-1d08-4923-b355-d8822f1eb6d4', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c201a6a-1d08-4923-b355-d8822f1eb6d4', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c54fab3-de7d-4008-b9b0-e6551a6ba741', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'9c54fab3-de7d-4008-b9b0-e6551a6ba741', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a0e6ce68-db29-4e77-a66e-003dd6549700', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a0e6ce68-db29-4e77-a66e-003dd6549700', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a0e6ce68-db29-4e77-a66e-003dd6549700', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8d36b9a-b358-40b1-b50a-85fb74066861', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8d36b9a-b358-40b1-b50a-85fb74066861', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', 3)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', 4)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a930c0b4-aa93-4ce6-8e1f-c32223dcc235', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'a930c0b4-aa93-4ce6-8e1f-c32223dcc235', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'aa4bd91d-813d-4dc2-b97f-842114320cc0', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'aa4bd91d-813d-4dc2-b97f-842114320cc0', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'aaee8e17-71a0-4a14-95b0-7598af1a756d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'aaee8e17-71a0-4a14-95b0-7598af1a756d', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'abeeddc3-485e-4b17-aa57-a80690c3a2a8', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'abeeddc3-485e-4b17-aa57-a80690c3a2a8', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'abeeddc3-485e-4b17-aa57-a80690c3a2a8', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ac64360e-f0ed-47d1-93eb-f525f5516055', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ac64360e-f0ed-47d1-93eb-f525f5516055', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ac64360e-f0ed-47d1-93eb-f525f5516055', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ac64360e-f0ed-47d1-93eb-f525f5516055', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ade76cfa-4791-475e-80fe-f30a7b697a6c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ade76cfa-4791-475e-80fe-f30a7b697a6c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ade76cfa-4791-475e-80fe-f30a7b697a6c', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ae6af645-bff2-44a6-9240-84fc173298a8', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ae6af645-bff2-44a6-9240-84fc173298a8', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b3eb99b2-e7f9-4a6e-92a9-37ea2b3e33b9', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b3eb99b2-e7f9-4a6e-92a9-37ea2b3e33b9', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b599c577-95d2-4ef3-8b2b-e9834d7496f8', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b599c577-95d2-4ef3-8b2b-e9834d7496f8', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b599c577-95d2-4ef3-8b2b-e9834d7496f8', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b624af56-772e-4b06-bde7-36fad49cb6a4', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b624af56-772e-4b06-bde7-36fad49cb6a4', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b83be517-9cc2-41bb-9d0f-a781d8af23bc', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b83be517-9cc2-41bb-9d0f-a781d8af23bc', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b83be517-9cc2-41bb-9d0f-a781d8af23bc', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b94d6994-5d48-4921-a15a-7d7112b61162', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'b94d6994-5d48-4921-a15a-7d7112b61162', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'be91bd18-ffbf-48ab-baee-1580743362bd', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'be91bd18-ffbf-48ab-baee-1580743362bd', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'be91bd18-ffbf-48ab-baee-1580743362bd', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'bf8d0fd1-2900-4b6d-a976-92ac0b9ceb29', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'bf8d0fd1-2900-4b6d-a976-92ac0b9ceb29', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c0f83376-8153-46cb-88d1-3e29f4c79bf1', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c0f83376-8153-46cb-88d1-3e29f4c79bf1', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c0f83376-8153-46cb-88d1-3e29f4c79bf1', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 3)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 4)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c9b22b04-a8fb-428d-aad5-9f9cd4bc184d', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'c9b22b04-a8fb-428d-aad5-9f9cd4bc184d', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cc038651-11f7-4b97-a1b1-f51b9970fed7', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cc038651-11f7-4b97-a1b1-f51b9970fed7', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cc0e96d4-8015-4425-9500-04d721a7a626', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cc0e96d4-8015-4425-9500-04d721a7a626', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cdd6a593-4098-4cd1-931d-61b2a3ecfb36', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cdd6a593-4098-4cd1-931d-61b2a3ecfb36', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ce2085e0-a5c7-4386-9164-bc6b031a9537', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cec57bc8-6f58-4338-bfe4-5edd04179056', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'cec57bc8-6f58-4338-bfe4-5edd04179056', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd4ffcf11-59f1-4760-a2fb-d912764679fe', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd4ffcf11-59f1-4760-a2fb-d912764679fe', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd4ffcf11-59f1-4760-a2fb-d912764679fe', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd558f827-e8ac-4101-aada-c9ed2ba9e6f6', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd558f827-e8ac-4101-aada-c9ed2ba9e6f6', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd670247e-863d-4cf7-a4d0-157e4aa87094', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd670247e-863d-4cf7-a4d0-157e4aa87094', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd670247e-863d-4cf7-a4d0-157e4aa87094', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'd6b9e14c-c628-4db7-942d-f44a3f631686', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dbb589ef-44e4-47ba-a960-e8a6bfc6ab3c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dbb589ef-44e4-47ba-a960-e8a6bfc6ab3c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dbe4797c-dac6-4797-8130-8bb40263412f', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dbe4797c-dac6-4797-8130-8bb40263412f', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'de97e813-3f99-488c-8cc7-c3d34522cc18', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'de97e813-3f99-488c-8cc7-c3d34522cc18', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dff1343d-1600-4ff7-ad2e-079f1fd5a056', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'dff1343d-1600-4ff7-ad2e-079f1fd5a056', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e054d4b9-279c-45ea-b83a-2c0544ecf779', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e054d4b9-279c-45ea-b83a-2c0544ecf779', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e10d2d13-d534-453c-90e2-7bef66e9e591', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e10d2d13-d534-453c-90e2-7bef66e9e591', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e188b6df-78c7-4044-bb76-1417ca318b7b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e188b6df-78c7-4044-bb76-1417ca318b7b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e51d5b0f-f711-473e-a4d6-3a33e5b2f137', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e51d5b0f-f711-473e-a4d6-3a33e5b2f137', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', 3)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e9c33f7c-0151-4dc3-913f-83d4069cc980', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e9c33f7c-0151-4dc3-913f-83d4069cc980', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e9dbff1d-89b0-4c53-af9d-99cd47d3c701', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'e9dbff1d-89b0-4c53-af9d-99cd47d3c701', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ea09d098-b44b-4aa4-b109-0426bb5b56bd', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ea09d098-b44b-4aa4-b109-0426bb5b56bd', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ecc23452-61c3-4f88-8648-c9b5c6191156', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ecc23452-61c3-4f88-8648-c9b5c6191156', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ecd79f59-4033-4f28-955d-8b3c15caf9af', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ecd79f59-4033-4f28-955d-8b3c15caf9af', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ecd79f59-4033-4f28-955d-8b3c15caf9af', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'eef183f9-9c4a-43b3-8584-0622c58d4f17', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'eef183f9-9c4a-43b3-8584-0622c58d4f17', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0eac1b1-8459-4f80-afe1-be1be4f0b20b', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0eac1b1-8459-4f80-afe1-be1be4f0b20b', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0f38b46-0a3b-4f7b-9c26-467c3a09c89a', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0f38b46-0a3b-4f7b-9c26-467c3a09c89a', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0f6f03b-e3bd-4dba-9560-0e69378e96a2', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f0f6f03b-e3bd-4dba-9560-0e69378e96a2', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f1ea47c0-a931-493b-b1c5-97dbb66f7c61', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f1ea47c0-a931-493b-b1c5-97dbb66f7c61', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 3)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 4)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 10)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', 11)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f4276411-a993-407d-90ef-a474d0b25d59', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'f4276411-a993-407d-90ef-a474d0b25d59', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fb679a26-0bf7-4d3c-81c4-94fbc4a175d5', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fb679a26-0bf7-4d3c-81c4-94fbc4a175d5', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fd590673-ce81-41fe-bab9-273fbe46a816', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fd590673-ce81-41fe-bab9-273fbe46a816', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fe9a6e01-9c02-4381-8473-8055f957adcd', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'fe9a6e01-9c02-4381-8473-8055f957adcd', 2)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ff85b2ce-68a0-405f-b52d-0ac5465c213c', 1)
GO
INSERT [dbo].[ApplicationRoleGroups] ([RoleId], [GroupId]) VALUES (N'ff85b2ce-68a0-405f-b52d-0ac5465c213c', 2)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', 11)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', 11)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', 11)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', 1)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', 2)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', 1)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', 2)
GO
INSERT [dbo].[ApplicationUserGroups] ([UserId], [GroupId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', 11)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'003052bc-baef-4034-909e-0fe93316cd3b', N'StateMasterEdit', N'Can Edit StateMaster', N'ApplicationRole', N'State Master', N'StateMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'008e4ba4-4491-4a60-92ff-837c92d5a533', N'PurchaseInvoicePrint', N'Can Print PurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoice', 26, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'009823bc-1fab-4a4c-a791-d86cf8b3b5e8', N'CustomerMasterIndex', N'Can View CustomerMaster', N'ApplicationRole', N'Customer Master', N'CustomerMaster', 14, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'04db2450-b80a-40bd-bef1-19629c053c49', N'BranchPurchaseInvoiceReports', N'Can View BranchPurchaseInvoice Report Details', N'ApplicationRole', N'Purchase Invoice', N'BranchPurchaseInvoice', 2, 3, N'Index', 1, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'05e00c77-9221-4b2b-a5e5-2bbd2ac6ca7c', N'UnitMasterCreate', N'Can Add UnitMaster', N'ApplicationRole', N'Unit', N'UnitMaster', 9, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'089f90cc-ee43-41ae-b6a5-bc3f687d96f5', N'HSNCodeMasterEdit', N'Can Edit HSNCode', N'ApplicationRole', N'HSNCode', N'HSNCodeMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'0b6a7382-f16f-42a0-8709-e6e5479bad3f', N'DesignationMasterEdit', N'Can Edit Designation', N'ApplicationRole', N'Designation', N'DesignationMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'0b8feab9-e7b0-4911-9c69-4fa7b293146f', N'BranchClientOrderIndex', N'Can View BranchClientOrder', N'ApplicationRole', N'Client Order', N'BranchClientOrder', 3, 7, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'13f357b1-f159-4f39-8d43-46c60d7e721c', N'PackingMasterDelete', N'Can Delete PackingMaster', N'ApplicationRole', N'Packing Master', N'PackingMaster', 21, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1817aa37-97cf-4a2e-8f3d-252f6d70737c', N'BranchStoreOpeningIndex', N'Can View BranchStoreOpening', N'ApplicationRole', N'BranchStore Opening', N'BranchStoreOpening', 1, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1b3a1693-a94d-48c0-95e7-83cdb27600df', N'MaterialMasterDelete', N'Can Delete MaterialMaster', N'ApplicationRole', N'Material Master', N'MaterialMaster', 6, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1b456eec-f457-4b14-a41a-f5625f20c98c', N'PackingMasterIndex', N'Can View PackingMaster', N'ApplicationRole', N'Packing Master', N'PackingMaster', 21, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1c0e1406-2911-4faa-a069-9e68656b439e', N'HOSalesInvoiceEdit', N'Can Edit HOSalesInvoice', N'ApplicationRole', N'HO Sales Invoice', N'HOSalesInvoice', 23, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1d611ac4-1563-4b82-a69a-0e2345412818', N'BranchMISRpt', N'Can View Branch Store MIS Report', N'ApplicationRole', N'MIS - Branch', N'BranchMISRpt', 12, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1e262a0a-eecf-4f5e-ae48-275a21443c41', N'BranchStoreStockViewIndex', N'Can View BranchStoreOpening Reports', N'ApplicationRole', N'Branch Stock', N'BranchStoreStockView', 24, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1f190114-62bf-4659-b560-35726eba9bd5', N'BranchMasterPrint', N'Can Print BranchMaster', N'ApplicationRole', N'Branch Master', N'BranchMaster', 2, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'1f882a20-1de0-4c2b-8f76-d675639e1668', N'FAOpeningCreate', N'Can View FAOpeningCreate', N'ApplicationRole', NULL, NULL, NULL, NULL, NULL, 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'224719a8-1dda-46b2-b4a5-061df4fbbff6', N'PurchaseOrderPrint', N'Can Print PurchaseOrder', N'ApplicationRole', N'Purchase Order', N'PurchaseOrder', 24, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'227ade1c-d9e3-473f-8949-c77264bbba45', N'DesignationMasterDelete', N'Can Delete Designation', N'ApplicationRole', N'Designation', N'DesignationMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'252c5ccf-1cca-4128-9fc6-28a2300e263a', N'DesignationMasterCreate', N'Can Create Designation', N'ApplicationRole', N'Designation', N'DesignationMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'268bf640-c4f5-4689-b5e0-a182d3ae1791', N'MainStoreOpeningEdit', N'Can Edit MainStoreOpening', N'ApplicationRole', N'HO Opening', N'MainStoreOpening', 32, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'273fe8d9-3431-43da-b7e2-b3aa5dae5375', N'BranchSalesInvoiceReturnDelete', N'Can Delete BranchSalesInvoiceReturn', N'ApplicationRole', N'Sales Return', N'BranchSalesInvoiceReturn', 23, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'27a0a33e-9d91-4b0c-8074-ad54e82c357a', N'BranchSalesInvoicePrint', N'Can Print BranchSalesInvoice', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'27ba93d5-b6c6-4e5d-9129-e88e7478b39e', N'PurchaseOrderEdit', N'Can Edit PurchaseOrder', N'ApplicationRole', N'Purchase Order', N'PurchaseOrder', 24, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'295cc85f-cb67-4207-bd5c-aff996c50635', N'HOSalesInvoiceCreate', N'Can Create HOSalesInvoice', N'ApplicationRole', N'HO Sales Invoice', N'HOSalesInvoice', 23, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'29d4bd40-1a56-47da-a10b-5ee676cba147', N'GoodsReceiptNoteCreate', N'Can Add GoodsReceiptNote', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNote', 25, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2aa40266-657d-46b8-88bf-9d87ca5cdb9f', N'MaterialGroupMasterEdit', N'Can Edit MaterialGroupMaster', N'ApplicationRole', N'MaterialGroup Master', N'MaterialGroupMaster', 5, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2b6bdf46-3ac2-4cc0-bda8-e0ec5be01bd4', N'CustomerMasterEdit', N'Can Edit CustomerMaster', N'ApplicationRole', N'Customer Master', N'CustomerMaster', 14, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2c99a770-e7d4-45a5-ac84-b36bab74ecda', N'BranchStoreStockRpt', N'Can View Branch Store Stock Rpt', N'ApplicationRole', N'Branch Store Stock', N'BranchStoreStockRpt', 10, 3, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2dd329dd-75bc-43b7-baba-5edde4db5f78', N'RateCardMasterIndex', N'Can View RateCardMaster', N'ApplicationRole', N'RateCardMaster', N'RateCardMaster', 7, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2ea983bc-2514-43b3-82a9-268ef1d654ed', N'PurchaseOrderDelete', N'Can Delete PurchaseOrder', N'ApplicationRole', N'Purchase Order', N'PurchaseOrder', 24, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2eed8755-5ad9-430f-a270-020e89c2245c', N'BranchMasterEdit', N'Can Edit BranchMaster', N'ApplicationRole', N'Branch Master', N'BranchMaster', 2, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'2f80ac4b-354c-43ee-866e-daca175146b7', N'CompanyMasterDelete', N'Can Delete CompanyMaster', N'ApplicationRole', N'Company Master', N'CompanyMaster', 1, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'3154d802-3802-4f6b-826b-5b153f50aadd', N'HSNCodeMasterCreate', N'Can Create HSNCode', N'ApplicationRole', N'HSNCode', N'HSNCodeMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'346e8175-8527-440c-85d6-d4f62039a609', N'BranchPurchaseInvoiceView', N'Can View BranchPurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'BranchPurchaseInvoice', 2, 7, N'Index', 1, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'349d82b5-2e2e-4425-a355-4a222a4f6e2e', N'GoodsReceiptNoteEdit', N'Can Edit GoodsReceiptNote', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNote', 25, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'3929e6c2-650c-4ecd-9621-02636fecf01d', N'CostFactorMasterIndex', N'Can View CostFactorMaster Index', N'ApplicationRole', N'Cost Factor', N'CostFactorMaster', 3, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'3b987631-fceb-4a44-a292-bcba10f23503', N'PurchaseInvoiceDelete', N'Can Delete PurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoice', 26, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'3c605d07-96ce-49a1-b43c-7b9b4646f6b7', N'MaterialMasterEdit', N'Can Edit MaterialMaster', N'ApplicationRole', N'Material Master', N'MaterialMaster', 6, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'3e1ff3c9-1813-4836-9fa6-da61161cc9df', N'CustomerMasterCreate', N'Can add CustomerMaster', N'ApplicationRole', N'Customer Master', N'CustomerMaster', 14, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'41050157-8f6b-413e-a389-9cab38766b89', N'HOSalesInvoiceIndex', N'Can Index HOSalesInvoiceIndex', N'ApplicationRole', N'HO Sales Invoice', N'HOSalesInvoice', 23, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'41ebd7fc-3372-41b9-a4cf-7db380328621', N'LocationMasterEdit', N'Can Edit LocationMaster', N'ApplicationRole', N'Location Master', N'LocationMaster', 11, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'4304038b-dcd5-45fa-a752-fed8e74990e6', N'MainStoreOpeningDelete', N'Can Delete MainStoreOpening', N'ApplicationRole', N'HO Opening', N'MainStoreOpening', 32, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'434e4bef-8678-4ef3-9819-cf16dc0538a2', N'GoodsReceiptNotePrint', N'Can Print GoodsReceiptNote', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNote', 25, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'45477ed9-faf0-4ecd-806e-12a66e5b182c', N'BranchSalesInvoiceReturnCreate', N'Can Create BranchSalesInvoiceReturn', N'ApplicationRole', N'Sales Return', N'BranchSalesInvoiceReturn', 23, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'46a33275-57a3-41ea-b43c-1df73d5cbba0', N'BranchTransferInvoiceCreate', N'Can Create BranchTransfer', N'ApplicationRole', N'Branch Transfer', N'BranchTransfer', 22, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'47665c5a-21d9-4194-b079-92e913af2683', N'CategoryMasterIndex', N'Can View CategoryMaster', N'ApplicationRole', N'Category Master', N'CategoryMaster', 4, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'47895b55-cd23-4b8c-96ce-7a8c721c7501', N'EmployeeMasterDelete', N'Can Delete EmployeeMaster', N'ApplicationRole', N'Employee Master', N'EmployeeMaster', 15, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'48360af1-14ca-444b-b6bd-8fecf889e29f', N'StateMasterCreate', N'Can Create StateMaster', N'ApplicationRole', N'State Master', N'StateMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'4a51ee63-c8fe-41f3-bd26-d19eeac8e78d', N'CategoryMasterDelete', N'Can Delete CategoryMaster', N'ApplicationRole', N'Category Master', N'CategoryMaster', 4, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'4f8122ae-2f21-4419-bee5-78e2e86c8fec', N'HSNCodeMasterIndex', N'Can View HSNCode', N'ApplicationRole', N'HSNCode', N'HSNCodeMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'52ae9049-716b-40ac-bdd8-595b2ff72818', N'BranchTransferInvoiceDelete', N'Can Delete BranchTransfer', N'ApplicationRole', N'Branch Transfer', N'BranchTransfer', 22, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'55b5a43a-b21e-45c0-9112-1c9ffb361ffd', N'MaterialGroupMasterDelete', N'Can Delete MaterialGroupMaster', N'ApplicationRole', N'MaterialGroup Master', N'MaterialGroupMaster', 5, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'58fc4203-c222-4695-8aa1-e3009578c091', N'MaterialGroupMasterIndex', N'Can View MaterialGroupMaster', N'ApplicationRole', N'MaterialGroup Master', N'MaterialGroupMaster', 5, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'5c5aec04-88da-4b84-80f6-83f278a3c316', N'UnitMasterIndex', N'Can View UnitMaster Index', N'ApplicationRole', N'Unit', N'UnitMaster', 9, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'5ff43aa9-4375-442b-8dfc-aad5e98e711e', N'SupplierMasterDelete', N'Can Delete SupplierMaster', N'ApplicationRole', N'SupplierMaster', N'SupplierMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'62761c93-fb91-4cca-a40d-096cd4e8e414', N'MainStoreOpeningCreate', N'Can Create MainStoreOpening', N'ApplicationRole', N'HO Opening', N'MainStoreOpening', 32, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'639535ed-a9d1-48ed-a336-618c9403b160', N'GoodsReceiptNoteReports', N'Can View GoodsReceiptNote Reports', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNoteRpt', 1, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'664c641a-b623-4b78-8859-ab93e708dcec', N'RateCardMasterDelete', N'Can Delete RateCardMaster', N'ApplicationRole', N'RateCardMaster', N'RateCardMaster', 7, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'66bce6fe-4480-4b3b-b291-27f0a6adc189', N'UnitMasterEdit', N'Can Edit UnitMaster', N'ApplicationRole', N'Unit', N'UnitMaster', 9, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'68702ee4-9ba1-484a-b7da-bbe63ff92534', N'EmployeeMasterIndex', N'Can View EmployeeMaster', N'ApplicationRole', N'Employee Master', N'EmployeeMaster', 15, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'68b75beb-898d-4287-8038-629da9609c84', N'CategoryMasterEdit', N'Can Edit CategoryMaster', N'ApplicationRole', N'Category Master', N'CategoryMaster', 4, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'68c48e9a-735d-44b1-8a86-f174d9e188e7', N'PurchaseOrderIndex', N'Can View PurchaseOrder Index', N'ApplicationRole', N'Purchase Order', N'PurchaseOrder', 24, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'6b63e9f5-3988-4bd8-a00a-46ad924835d6', N'RateCardMasterCreate', N'Can add RateCardMaster', N'ApplicationRole', N'RateCardMaster', N'RateCardMaster', 7, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d', N'BranchPurchaseInvoiceIndex', N'Can View BranchPurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'BranchPurchaseInvoice', 2, 7, N'Index', 1, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'73a06379-5875-4ff1-a68a-db11d095cd51', N'DealerTargetPlanCreate', N'Can Create Dealer Target Plan', N'ApplicationRole', N'Dealer Target Plan Vs. Actuals', N'DealerTargetPlan', 2, 8, N'Index', 1, N'fa fa-bullseye')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'7487606c-b996-4157-8632-2e0b7d76ce29', N'CompanyMasterCreate', N'Can Add CompanyMaster', N'ApplicationRole', N'Company Master', N'CompanyMaster', 1, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'75facf1c-374d-48c9-8141-d6913051deb0', N'PurchaseInvoiceEdit', N'Can Edit PurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoice', 26, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'760dd5f6-dd3f-4db9-a7a8-c5279921e5ec', N'PurchaseInvoiceIndex', N'Can View PurchaseInvoice Index', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoice', 26, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'76635a55-43b8-4e39-bf0a-ef56076680c6', N'SupplierMasterIndex', N'Can View SupplierMaster', N'ApplicationRole', N'SupplierMaster', N'SupplierMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'778323a9-a367-4da0-aaeb-444ff55bfd4b', N'StateMasterIndex', N'Can View StateMaster', N'ApplicationRole', N'State Master', N'StateMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98', N'EmployeeMasterEdit', N'Can Edit EmployeeMaster', N'ApplicationRole', N'Employee Master', N'EmployeeMaster', 15, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'8198891b-6206-42c6-8ae1-afeba734946b', N'CostFactorMasterCreate', N'Can Add CostFactorMaster', N'ApplicationRole', N'Cost Factor', N'CostFactorMaster', 3, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'83f11971-94d7-4d00-aaf6-362ac489ad60', N'DepartmentMasterIndex', N'Can View DepartmentMaster', N'ApplicationRole', N'Department Master', N'DepartmentMaster', 4, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'893342df-99f4-422d-804a-1e1f98700547', N'BranchSalesInvoiceReturnEdit', N'Can Edit BranchSalesInvoiceReturn', N'ApplicationRole', N'Sales Return', N'BranchSalesInvoiceReturn', 23, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'8ada39fb-92f0-41be-a0b3-1b4aed28990b', N'BranchStoreOpeningDelete', N'Can Delete BranchStoreOpening', N'ApplicationRole', N'BranchStore Opening', N'BranchStoreOpening', 1, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9171a4b8-5349-4b48-93a6-12bacf724009', N'BranchClientOrderEdit', N'Can Edit BranchClientOrder', N'ApplicationRole', N'Client Order', N'BranchClientOrder', 3, 7, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9226ef62-6122-4c86-9e7e-7765ee64bdb6', N'LocationMasterDelete', N'Can Delete LocationMaster', N'ApplicationRole', N'Location Master', N'LocationMaster', 11, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'96a18b20-032e-42b7-8d2c-9ec39aa978a0', N'ROLReports', N'Can View ROL Reports', N'ApplicationRole', N'MIS - ROL', N'ROLMISRpt', 13, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9bbdc56c-4994-49c3-960b-9e4b59e6fc2d', N'MaterialMasterCreate', N'Can Add MaterialMaster', N'ApplicationRole', N'Material Master', N'MaterialMaster', 6, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9bc02953-e816-490e-aa24-907871b5bd89', N'BranchSalesInvoiceDelete', N'Can Delete BranchSalesInvoice', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9c17d10e-25d0-4dab-b28a-b3b11972a33d', N'CompanyMasterEdit', N'Can Edit CompanyMaster', N'ApplicationRole', N'Company Master', N'CompanyMaster', 1, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9c201a6a-1d08-4923-b355-d8822f1eb6d4', N'RateCardMasterEdit', N'Can Edit RateCardMaster', N'ApplicationRole', N'RateCardMaster', N'RateCardMaster', 7, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'9c54fab3-de7d-4008-b9b0-e6551a6ba741', N'CostFactorMasterDelete', N'Can Delete CostFactorMaster', N'ApplicationRole', N'Cost Factor', N'CostFactorMaster', 3, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'a0e6ce68-db29-4e77-a66e-003dd6549700', N'BranchClientOrderCreate', N'Can add BranchClientOrder', N'ApplicationRole', N'Client Order', N'BranchClientOrder', 3, 7, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9', N'BranchSalesInvoiceReturnPrint', N'Can Print BranchSalesInvoiceReturn', N'ApplicationRole', N'Sales Return', N'BranchSalesInvoiceReturn', 23, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'a8d36b9a-b358-40b1-b50a-85fb74066861', N'BranchTransferInvoiceEdit', N'Can Edit BranchTransfer', N'ApplicationRole', N'Branch Transfer', N'BranchTransfer', 22, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'a8fbf935-32f6-4905-9b25-0af55a706bb2', N'Sales_EInvoice_Index', N'Can View Sales_EInvoice_Index', N'ApplicationRole', N'E-Invoice', N'SalesEInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-bullhorn')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'a930c0b4-aa93-4ce6-8e1f-c32223dcc235', N'MainStoreStockViewIndex', N'Can View MainStoreStockViewIndex', N'ApplicationRole', N'MainStoreStockView', N'MainStoreStockView', 15, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'aa4bd91d-813d-4dc2-b97f-842114320cc0', N'CostFactorMasterEdit', N'Can Edit CostFactorMaster', N'ApplicationRole', N'Cost Factor', N'CostFactorMaster', 3, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'aa9c1271-5cb9-4c2b-a828-59088a842bfe', N'PackingMasterCreate', N'Can Add PackingMaster', N'ApplicationRole', N'Packing Master', N'PackingMaster', 21, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'aaee8e17-71a0-4a14-95b0-7598af1a756d', N'BranchTransferInvoicePrint', N'Can Print BranchTransfer', N'ApplicationRole', N'Branch Transfer', N'BranchTransfer', 22, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8', N'EmployeeMasterCreate', N'Can Add EmployeeMaster', N'ApplicationRole', N'Employee Master', N'EmployeeMaster', 15, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'abeeddc3-485e-4b17-aa57-a80690c3a2a8', N'BranchPurchaseInvoicePrint', N'Can Print BranchPurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'BranchPurchaseInvoice', 2, 7, N'Index', 1, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ac64360e-f0ed-47d1-93eb-f525f5516055', N'DealerTargetPlanIndex', N'Can View Dealer Target Plan', N'ApplicationRole', N'Dealer Target Plan Vs. Actuals', N'DealerTargetPlan', 2, 8, N'Index', 1, N'fa fa-bullseye')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ade76cfa-4791-475e-80fe-f30a7b697a6c', N'BranchSalesInvoiceEdit', N'Can Edit BranchSalesInvoice', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ae6af645-bff2-44a6-9240-84fc173298a8', N'BranchTransferInvoiceIndex', N'Can View BranchTransfer', N'ApplicationRole', N'Branch Transfer', N'BranchTransfer', 22, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'b3eb99b2-e7f9-4a6e-92a9-37ea2b3e33b9', N'PurchaseOrderReports', N'Can View Purchase Order Reports', N'ApplicationRole', N'Purchase Order', N'PurchaseOrderRpt', 2, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'b599c577-95d2-4ef3-8b2b-e9834d7496f8', N'BranchSalesInvoiceReturnIndex', N'Can View BranchSalesInvoiceReturn', N'ApplicationRole', N'Sales Return', N'BranchSalesInvoiceReturn', 23, 7, N'Index', 1, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'b624af56-772e-4b06-bde7-36fad49cb6a4', N'HOSalesInvoicePrint', N'Can Print HOSalesInvoice', N'ApplicationRole', N'HO Sales Invoice', N'HOSalesInvoice', 23, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'b83be517-9cc2-41bb-9d0f-a781d8af23bc', N'BranchClientOrderDelete', N'Can Delete BranchClientOrder', N'ApplicationRole', N'Client Order', N'BranchClientOrder', 3, 7, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'b94d6994-5d48-4921-a15a-7d7112b61162', N'LocationMasterIndex', N'Can View LocationMaster', N'ApplicationRole', N'Location Master', N'LocationMaster', 11, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'be91bd18-ffbf-48ab-baee-1580743362bd', N'BranchStoreOpeningEdit', N'Can Edit BranchStoreOpening', N'ApplicationRole', N'BranchStore Opening', N'BranchStoreOpening', 1, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'bf8d0fd1-2900-4b6d-a976-92ac0b9ceb29', N'DepartmentMasterEdit', N'Can Edit DepartmentMaster', N'ApplicationRole', N'Department Master', N'DepartmentMaster', 4, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'c0f83376-8153-46cb-88d1-3e29f4c79bf1', N'BranchStoreOpeningPrint', N'Can Print BranchStoreOpening', N'ApplicationRole', N'BranchStore Opening', N'BranchStoreOpening', 1, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'c2767418-80ce-4e95-8217-725734180918', N'BranchSalesInvoiceCreate', N'Can Add BranchSalesInvoice', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'c9b22b04-a8fb-428d-aad5-9f9cd4bc184d', N'UnitMasterDelete', N'Can Delete UnitMaster', N'ApplicationRole', N'Unit', N'UnitMaster', 9, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'cc038651-11f7-4b97-a1b1-f51b9970fed7', N'CustomerMasterDelete', N'Can Delete CustomerMaster', N'ApplicationRole', N'Customer Master', N'CustomerMaster', 14, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'cc0e96d4-8015-4425-9500-04d721a7a626', N'DepartmentMasterDelete', N'Can Delete DepartmentMaster', N'ApplicationRole', N'Department Master', N'DepartmentMaster', 4, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'cdd6a593-4098-4cd1-931d-61b2a3ecfb36', N'BranchMasterCreate', N'Can Add BranchMaster', N'ApplicationRole', N'Branch Master', N'BranchMaster', 2, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ce2085e0-a5c7-4386-9164-bc6b031a9537', N'MaterialRequestApprove', N'Can Approve MaterialRequest', N'ApplicationRole', NULL, NULL, NULL, NULL, NULL, 0, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'cec57bc8-6f58-4338-bfe4-5edd04179056', N'GoodsReceiptNoteDelete', N'Can Delete GoodsReceiptNote', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNote', 25, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'd1c9cd27-729a-4d63-b585-0b1166a965d0', N'PackingMasterEdit', N'Can Edit PackingMaster', N'ApplicationRole', N'Packing Master', N'PackingMaster', 21, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'd4ffcf11-59f1-4760-a2fb-d912764679fe', N'BranchClientOrderReports', N'Can View BranchClientOrder Reports', N'ApplicationRole', N'Client Order', N'BranchClientOrderRpt', 33, 33, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'd558f827-e8ac-4101-aada-c9ed2ba9e6f6', N'GoodsReceiptNoteIndex', N'Can View GoodsReceiptNote', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNote', 25, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'd670247e-863d-4cf7-a4d0-157e4aa87094', N'BranchClientOrderPrint', N'Can Print BranchClientOrder', N'ApplicationRole', N'Client Order', N'BranchClientOrder', 3, 7, N'Index', 1, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'd6b9e14c-c628-4db7-942d-f44a3f631686', N'Home', N'Add, modify, and delete Groups', N'ApplicationRole', N'Home', N'Home', 1, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'dbb589ef-44e4-47ba-a960-e8a6bfc6ab3c', N'BranchMasterDelete', N'Can Delete BranchMaster', N'ApplicationRole', N'Branch Master', N'BranchMaster', 2, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'dbe4797c-dac6-4797-8130-8bb40263412f', N'HSNCodeMasterDelete', N'Can Delete HSNCode', N'ApplicationRole', N'HSNCode', N'HSNCodeMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'de97e813-3f99-488c-8cc7-c3d34522cc18', N'DealerTargetPlanDelete', N'Can Delete Dealer Target Plan', N'ApplicationRole', N'Dealer Target Plan Vs. Actuals', N'DealerTargetPlan', 2, 8, N'Index', 1, N'fa fa-bullseye')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'dff1343d-1600-4ff7-ad2e-079f1fd5a056', N'LocationMasterCreate', N'Can Create LocationMaster', N'ApplicationRole', N'Location Master', N'LocationMaster', 11, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e054d4b9-279c-45ea-b83a-2c0544ecf779', N'SupplierMasterEdit', N'Can Edit SupplierMaster', N'ApplicationRole', N'SupplierMaster', N'SupplierMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e0e2d2ba-11f6-4c2d-988e-0d8bc61809ab', N'GoodsReceiptNoteReports', N'Can View GoodsReceiptNote Reports', N'ApplicationRole', N'GoodsReceiptNote', N'GoodsReceiptNoteRpt', 3, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e10d2d13-d534-453c-90e2-7bef66e9e591', N'CostFactorMasterPrint', N'Can Print CostFactorMaster', N'ApplicationRole', N'Cost Factor', N'CostFactorMaster', 3, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e188b6df-78c7-4044-bb76-1417ca318b7b', N'SupplierMasterCreate', N'Can Add SupplierMaster', N'ApplicationRole', N'SupplierMaster', N'SupplierMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e51d5b0f-f711-473e-a4d6-3a33e5b2f137', N'BranchMasterIndex', N'Can View BranchMaster', N'ApplicationRole', N'Branch Master', N'BranchMaster', 2, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e64b763f-a71c-4da8-bc4e-de876b88a983', N'MISRpt', N'Can View Main Store MIS Report', N'ApplicationRole', N'MIS - Main Store', N'MISRpt', 11, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e94398bf-0033-4bc4-a713-fbad25de711c', N'BranchSalesInvoiceReports', N'Can View BranchSalesInvoice Reports', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoiceRpt', 34, 33, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e9c33f7c-0151-4dc3-913f-83d4069cc980', N'DesignationMasterIndex', N'Can View Designation', N'ApplicationRole', N'GatePass', N'DesignationMaster', 8, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'e9dbff1d-89b0-4c53-af9d-99cd47d3c701', N'CategoryMasterCreate', N'Can Create CategoryMaster', N'ApplicationRole', N'Category Master', N'CategoryMaster', 4, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ea09d098-b44b-4aa4-b109-0426bb5b56bd', N'PurchaseInvoiceReports', N'Can View Purchase Invoice Reports', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoiceRpt', 4, 3, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ecc23452-61c3-4f88-8648-c9b5c6191156', N'StateMasterDelete', N'Can Delete StateMaster', N'ApplicationRole', N'State Master', N'StateMaster', 10, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ecd79f59-4033-4f28-955d-8b3c15caf9af', N'BranchStoreOpeningCreate', N'Can Add BranchStoreOpening', N'ApplicationRole', N'BranchStore Opening', N'BranchStoreOpening', 1, 7, N'Index', 1, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'eef183f9-9c4a-43b3-8584-0622c58d4f17', N'PurchaseInvoiceCreate', N'Can Add PurchaseInvoice', N'ApplicationRole', N'Purchase Invoice', N'PurchaseInvoice', 26, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f0eac1b1-8459-4f80-afe1-be1be4f0b20b', N'Admin', N'Global Access', N'ApplicationRole', NULL, NULL, NULL, NULL, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f0f38b46-0a3b-4f7b-9c26-467c3a09c89a', N'MainStoreOpeningPrint', N'Can Print MainStoreOpening', N'ApplicationRole', N'PurchaseIndent', N'MainStoreOpening', 32, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f0f6f03b-e3bd-4dba-9560-0e69378e96a2', N'MainStoreOpeningIndex', N'Can View MainStoreOpening', N'ApplicationRole', N'HO Opening', N'MainStoreOpening', 32, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f1ea47c0-a931-493b-b1c5-97dbb66f7c61', N'CompanyMasterIndex', N'Can View CompanyMaster', N'ApplicationRole', N'Company Master', N'CompanyMaster', 1, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f31e91de-0ac0-4ce9-85aa-8253eb552e67', N'BranchSalesInvoiceIndex', N'Can View BranchSalesInvoice', N'ApplicationRole', N'Sales Invoice', N'BranchSalesInvoice', 21, 7, N'Index', 1, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'f4276411-a993-407d-90ef-a474d0b25d59', N'DepartmentMasterCreate', N'Can Add DepartmentMaster', N'ApplicationRole', N'Department Master', N'DepartmentMaster', 4, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'fb679a26-0bf7-4d3c-81c4-94fbc4a175d5', N'PurchaseOrderCreate', N'Can Add PurchaseOrder ', N'ApplicationRole', N'Purchase Order', N'PurchaseOrder', 24, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'fd590673-ce81-41fe-bab9-273fbe46a816', N'MaterialGroupMasterCreate', N'Can Add MaterialGroupMaster', N'ApplicationRole', N'MaterialGroup Master', N'MaterialGroupMaster', 5, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'fe9a6e01-9c02-4381-8473-8055f957adcd', N'HOSalesInvoiceDelete', N'Can Delete HOSalesInvoice', N'ApplicationRole', N'HO Sales Invoice', N'HOSalesInvoice', 23, 2, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [Description], [Discriminator], [RMenuType], [RControllerName], [RMenuGroupId], [RMenuGroupOrder], [RMenuIndex], [SDPTID], [RImageClassName]) VALUES (N'ff85b2ce-68a0-405f-b52d-0ac5465c213c', N'MaterialMasterIndex', N'Can View MaterialMaster', N'ApplicationRole', N'Material Master', N'MaterialMaster', 6, 1, N'Index', 1, NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'003052bc-baef-4034-909e-0fe93316cd3b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'008e4ba4-4491-4a60-92ff-837c92d5a533')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'009823bc-1fab-4a4c-a791-d86cf8b3b5e8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'05e00c77-9221-4b2b-a5e5-2bbd2ac6ca7c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'089f90cc-ee43-41ae-b6a5-bc3f687d96f5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'0b6a7382-f16f-42a0-8709-e6e5479bad3f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'1b3a1693-a94d-48c0-95e7-83cdb27600df')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'1c0e1406-2911-4faa-a069-9e68656b439e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'1f190114-62bf-4659-b560-35726eba9bd5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'224719a8-1dda-46b2-b4a5-061df4fbbff6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'227ade1c-d9e3-473f-8949-c77264bbba45')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'252c5ccf-1cca-4128-9fc6-28a2300e263a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'268bf640-c4f5-4689-b5e0-a182d3ae1791')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'27ba93d5-b6c6-4e5d-9129-e88e7478b39e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'295cc85f-cb67-4207-bd5c-aff996c50635')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'29d4bd40-1a56-47da-a10b-5ee676cba147')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2aa40266-657d-46b8-88bf-9d87ca5cdb9f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2b6bdf46-3ac2-4cc0-bda8-e0ec5be01bd4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2dd329dd-75bc-43b7-baba-5edde4db5f78')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2ea983bc-2514-43b3-82a9-268ef1d654ed')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2eed8755-5ad9-430f-a270-020e89c2245c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'2f80ac4b-354c-43ee-866e-daca175146b7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'3154d802-3802-4f6b-826b-5b153f50aadd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'349d82b5-2e2e-4425-a355-4a222a4f6e2e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'3929e6c2-650c-4ecd-9621-02636fecf01d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'3b987631-fceb-4a44-a292-bcba10f23503')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'3c605d07-96ce-49a1-b43c-7b9b4646f6b7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'3e1ff3c9-1813-4836-9fa6-da61161cc9df')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'41050157-8f6b-413e-a389-9cab38766b89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'41ebd7fc-3372-41b9-a4cf-7db380328621')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'4304038b-dcd5-45fa-a752-fed8e74990e6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'434e4bef-8678-4ef3-9819-cf16dc0538a2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'47665c5a-21d9-4194-b079-92e913af2683')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'47895b55-cd23-4b8c-96ce-7a8c721c7501')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'48360af1-14ca-444b-b6bd-8fecf889e29f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'4a51ee63-c8fe-41f3-bd26-d19eeac8e78d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'4f8122ae-2f21-4419-bee5-78e2e86c8fec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'55b5a43a-b21e-45c0-9112-1c9ffb361ffd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'58fc4203-c222-4695-8aa1-e3009578c091')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'5c5aec04-88da-4b84-80f6-83f278a3c316')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'5ff43aa9-4375-442b-8dfc-aad5e98e711e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'62761c93-fb91-4cca-a40d-096cd4e8e414')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'664c641a-b623-4b78-8859-ab93e708dcec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'66bce6fe-4480-4b3b-b291-27f0a6adc189')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'68702ee4-9ba1-484a-b7da-bbe63ff92534')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'68b75beb-898d-4287-8038-629da9609c84')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'68c48e9a-735d-44b1-8a86-f174d9e188e7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'6b63e9f5-3988-4bd8-a00a-46ad924835d6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'7487606c-b996-4157-8632-2e0b7d76ce29')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'75facf1c-374d-48c9-8141-d6913051deb0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'760dd5f6-dd3f-4db9-a7a8-c5279921e5ec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'76635a55-43b8-4e39-bf0a-ef56076680c6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'778323a9-a367-4da0-aaeb-444ff55bfd4b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'8198891b-6206-42c6-8ae1-afeba734946b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'83f11971-94d7-4d00-aaf6-362ac489ad60')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9226ef62-6122-4c86-9e7e-7765ee64bdb6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9bbdc56c-4994-49c3-960b-9e4b59e6fc2d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9c17d10e-25d0-4dab-b28a-b3b11972a33d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9c201a6a-1d08-4923-b355-d8822f1eb6d4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'9c54fab3-de7d-4008-b9b0-e6551a6ba741')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'a930c0b4-aa93-4ce6-8e1f-c32223dcc235')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'aa4bd91d-813d-4dc2-b97f-842114320cc0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'b3eb99b2-e7f9-4a6e-92a9-37ea2b3e33b9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'b624af56-772e-4b06-bde7-36fad49cb6a4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'b94d6994-5d48-4921-a15a-7d7112b61162')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'bf8d0fd1-2900-4b6d-a976-92ac0b9ceb29')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'c9b22b04-a8fb-428d-aad5-9f9cd4bc184d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'cc038651-11f7-4b97-a1b1-f51b9970fed7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'cc0e96d4-8015-4425-9500-04d721a7a626')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'cdd6a593-4098-4cd1-931d-61b2a3ecfb36')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'cec57bc8-6f58-4338-bfe4-5edd04179056')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'd558f827-e8ac-4101-aada-c9ed2ba9e6f6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'd6b9e14c-c628-4db7-942d-f44a3f631686')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'dbb589ef-44e4-47ba-a960-e8a6bfc6ab3c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'dbe4797c-dac6-4797-8130-8bb40263412f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'de97e813-3f99-488c-8cc7-c3d34522cc18')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'dff1343d-1600-4ff7-ad2e-079f1fd5a056')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e054d4b9-279c-45ea-b83a-2c0544ecf779')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e10d2d13-d534-453c-90e2-7bef66e9e591')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e188b6df-78c7-4044-bb76-1417ca318b7b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e51d5b0f-f711-473e-a4d6-3a33e5b2f137')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e9c33f7c-0151-4dc3-913f-83d4069cc980')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'e9dbff1d-89b0-4c53-af9d-99cd47d3c701')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ea09d098-b44b-4aa4-b109-0426bb5b56bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ecc23452-61c3-4f88-8648-c9b5c6191156')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'eef183f9-9c4a-43b3-8584-0622c58d4f17')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f0eac1b1-8459-4f80-afe1-be1be4f0b20b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f0f38b46-0a3b-4f7b-9c26-467c3a09c89a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f0f6f03b-e3bd-4dba-9560-0e69378e96a2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f1ea47c0-a931-493b-b1c5-97dbb66f7c61')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'f4276411-a993-407d-90ef-a474d0b25d59')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'fb679a26-0bf7-4d3c-81c4-94fbc4a175d5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'fd590673-ce81-41fe-bab9-273fbe46a816')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'fe9a6e01-9c02-4381-8473-8055f957adcd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'ff85b2ce-68a0-405f-b52d-0ac5465c213c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'003052bc-baef-4034-909e-0fe93316cd3b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'008e4ba4-4491-4a60-92ff-837c92d5a533')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'009823bc-1fab-4a4c-a791-d86cf8b3b5e8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'05e00c77-9221-4b2b-a5e5-2bbd2ac6ca7c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'089f90cc-ee43-41ae-b6a5-bc3f687d96f5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'0b6a7382-f16f-42a0-8709-e6e5479bad3f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'1b3a1693-a94d-48c0-95e7-83cdb27600df')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'1c0e1406-2911-4faa-a069-9e68656b439e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'1f190114-62bf-4659-b560-35726eba9bd5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'224719a8-1dda-46b2-b4a5-061df4fbbff6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'227ade1c-d9e3-473f-8949-c77264bbba45')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'252c5ccf-1cca-4128-9fc6-28a2300e263a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'268bf640-c4f5-4689-b5e0-a182d3ae1791')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'27ba93d5-b6c6-4e5d-9129-e88e7478b39e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'295cc85f-cb67-4207-bd5c-aff996c50635')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'29d4bd40-1a56-47da-a10b-5ee676cba147')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2aa40266-657d-46b8-88bf-9d87ca5cdb9f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2b6bdf46-3ac2-4cc0-bda8-e0ec5be01bd4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2dd329dd-75bc-43b7-baba-5edde4db5f78')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2ea983bc-2514-43b3-82a9-268ef1d654ed')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2eed8755-5ad9-430f-a270-020e89c2245c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'2f80ac4b-354c-43ee-866e-daca175146b7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'3154d802-3802-4f6b-826b-5b153f50aadd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'349d82b5-2e2e-4425-a355-4a222a4f6e2e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'3929e6c2-650c-4ecd-9621-02636fecf01d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'3b987631-fceb-4a44-a292-bcba10f23503')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'3c605d07-96ce-49a1-b43c-7b9b4646f6b7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'3e1ff3c9-1813-4836-9fa6-da61161cc9df')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'41050157-8f6b-413e-a389-9cab38766b89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'41ebd7fc-3372-41b9-a4cf-7db380328621')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'4304038b-dcd5-45fa-a752-fed8e74990e6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'434e4bef-8678-4ef3-9819-cf16dc0538a2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'47665c5a-21d9-4194-b079-92e913af2683')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'47895b55-cd23-4b8c-96ce-7a8c721c7501')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'48360af1-14ca-444b-b6bd-8fecf889e29f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'4a51ee63-c8fe-41f3-bd26-d19eeac8e78d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'4f8122ae-2f21-4419-bee5-78e2e86c8fec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'55b5a43a-b21e-45c0-9112-1c9ffb361ffd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'58fc4203-c222-4695-8aa1-e3009578c091')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'5c5aec04-88da-4b84-80f6-83f278a3c316')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'5ff43aa9-4375-442b-8dfc-aad5e98e711e')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'62761c93-fb91-4cca-a40d-096cd4e8e414')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'664c641a-b623-4b78-8859-ab93e708dcec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'66bce6fe-4480-4b3b-b291-27f0a6adc189')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'68702ee4-9ba1-484a-b7da-bbe63ff92534')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'68b75beb-898d-4287-8038-629da9609c84')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'68c48e9a-735d-44b1-8a86-f174d9e188e7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'6b63e9f5-3988-4bd8-a00a-46ad924835d6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'7487606c-b996-4157-8632-2e0b7d76ce29')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'75facf1c-374d-48c9-8141-d6913051deb0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'760dd5f6-dd3f-4db9-a7a8-c5279921e5ec')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'76635a55-43b8-4e39-bf0a-ef56076680c6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'778323a9-a367-4da0-aaeb-444ff55bfd4b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'7c8fcccb-d3b8-4e5b-bf46-1ea13f1dbd98')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'8198891b-6206-42c6-8ae1-afeba734946b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'83f11971-94d7-4d00-aaf6-362ac489ad60')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9226ef62-6122-4c86-9e7e-7765ee64bdb6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9bbdc56c-4994-49c3-960b-9e4b59e6fc2d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9c17d10e-25d0-4dab-b28a-b3b11972a33d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9c201a6a-1d08-4923-b355-d8822f1eb6d4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'9c54fab3-de7d-4008-b9b0-e6551a6ba741')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'a930c0b4-aa93-4ce6-8e1f-c32223dcc235')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'aa4bd91d-813d-4dc2-b97f-842114320cc0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ab721d39-1880-4c86-b6f1-2b4c9a7654b8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'b3eb99b2-e7f9-4a6e-92a9-37ea2b3e33b9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'b624af56-772e-4b06-bde7-36fad49cb6a4')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'b94d6994-5d48-4921-a15a-7d7112b61162')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'bf8d0fd1-2900-4b6d-a976-92ac0b9ceb29')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'c9b22b04-a8fb-428d-aad5-9f9cd4bc184d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'cc038651-11f7-4b97-a1b1-f51b9970fed7')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'cc0e96d4-8015-4425-9500-04d721a7a626')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'cdd6a593-4098-4cd1-931d-61b2a3ecfb36')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'cec57bc8-6f58-4338-bfe4-5edd04179056')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'd558f827-e8ac-4101-aada-c9ed2ba9e6f6')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'd6b9e14c-c628-4db7-942d-f44a3f631686')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'dbb589ef-44e4-47ba-a960-e8a6bfc6ab3c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'dbe4797c-dac6-4797-8130-8bb40263412f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'de97e813-3f99-488c-8cc7-c3d34522cc18')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'dff1343d-1600-4ff7-ad2e-079f1fd5a056')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e054d4b9-279c-45ea-b83a-2c0544ecf779')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e10d2d13-d534-453c-90e2-7bef66e9e591')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e188b6df-78c7-4044-bb76-1417ca318b7b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e51d5b0f-f711-473e-a4d6-3a33e5b2f137')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e9c33f7c-0151-4dc3-913f-83d4069cc980')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'e9dbff1d-89b0-4c53-af9d-99cd47d3c701')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ea09d098-b44b-4aa4-b109-0426bb5b56bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ecc23452-61c3-4f88-8648-c9b5c6191156')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'eef183f9-9c4a-43b3-8584-0622c58d4f17')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f0eac1b1-8459-4f80-afe1-be1be4f0b20b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f0f38b46-0a3b-4f7b-9c26-467c3a09c89a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f0f6f03b-e3bd-4dba-9560-0e69378e96a2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f1ea47c0-a931-493b-b1c5-97dbb66f7c61')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'f4276411-a993-407d-90ef-a474d0b25d59')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'fb679a26-0bf7-4d3c-81c4-94fbc4a175d5')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'fd590673-ce81-41fe-bab9-273fbe46a816')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'fe9a6e01-9c02-4381-8473-8055f957adcd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'ff85b2ce-68a0-405f-b52d-0ac5465c213c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'04db2450-b80a-40bd-bef1-19629c053c49')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'0b8feab9-e7b0-4911-9c69-4fa7b293146f')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'1817aa37-97cf-4a2e-8f3d-252f6d70737c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'1e262a0a-eecf-4f5e-ae48-275a21443c41')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'273fe8d9-3431-43da-b7e2-b3aa5dae5375')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'27a0a33e-9d91-4b0c-8074-ad54e82c357a')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'2c99a770-e7d4-45a5-ac84-b36bab74ecda')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'346e8175-8527-440c-85d6-d4f62039a609')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'45477ed9-faf0-4ecd-806e-12a66e5b182c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'46a33275-57a3-41ea-b43c-1df73d5cbba0')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'52ae9049-716b-40ac-bdd8-595b2ff72818')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'6fb1f7b2-5da0-4873-8e7a-fc6d2f6d361d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'73a06379-5875-4ff1-a68a-db11d095cd51')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'893342df-99f4-422d-804a-1e1f98700547')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'8ada39fb-92f0-41be-a0b3-1b4aed28990b')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'9171a4b8-5349-4b48-93a6-12bacf724009')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'9bc02953-e816-490e-aa24-907871b5bd89')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'a0e6ce68-db29-4e77-a66e-003dd6549700')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'a3ba54cf-d8e2-4173-8f46-a77d091e74d9')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'a8d36b9a-b358-40b1-b50a-85fb74066861')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'a8fbf935-32f6-4905-9b25-0af55a706bb2')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'aaee8e17-71a0-4a14-95b0-7598af1a756d')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'abeeddc3-485e-4b17-aa57-a80690c3a2a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'ac64360e-f0ed-47d1-93eb-f525f5516055')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'ade76cfa-4791-475e-80fe-f30a7b697a6c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'ae6af645-bff2-44a6-9240-84fc173298a8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'b599c577-95d2-4ef3-8b2b-e9834d7496f8')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'b83be517-9cc2-41bb-9d0f-a781d8af23bc')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'be91bd18-ffbf-48ab-baee-1580743362bd')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'c0f83376-8153-46cb-88d1-3e29f4c79bf1')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'c2767418-80ce-4e95-8217-725734180918')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'd4ffcf11-59f1-4760-a2fb-d912764679fe')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'd670247e-863d-4cf7-a4d0-157e4aa87094')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'e94398bf-0033-4bc4-a713-fbad25de711c')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'ecd79f59-4033-4f28-955d-8b3c15caf9af')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'f31e91de-0ac0-4ce9-85aa-8253eb552e67')
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'0571f04f-c1d5-45b7-a423-1cdebb8612a1', N'csr', N'AA/3HGIfwV17yGtzFsTFYe4knhKQ5k/Y/ypVNfzzm+3Nf+ONm0k23fFPr9/ZCBPFGg==', N'3a8f69c8-62cb-435e-baf1-03d46d6fdfe2', N'Chandra Shekhar', N'Reddy', N'kurnool@shefa.co.in', N'ApplicationUser', N'csr@123', 3, N'SHEFA AGRICARE TECHNOLOGIES PRIVATE LIMITED(Kurnool)', 0, NULL)
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'7037c1b2-4543-40fb-b379-03b230a310fc', N'suresh', N'AFsAmdyQQz4+/4DN96NLAjYDWNQiBV3dQGvhU01m5vwmNbBvUYhr0NzjqOSXKt0DjA==', N'b292f923-0d7e-459f-aa44-5c4457d2a990', N'suresh', N'suresh', N'suresh@fusiontec.com', N'ApplicationUser', N'123456', 3, N'Shefa Agricare Technologies(Kurnool)', 0, NULL)
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'c2807f5f-802c-4a7a-bf9d-ca297bea0600', N'dinesh', N'AH6vDB1oCx0Znu4xyA9/jeUJbotSDkmkwa3SYbAfRMq69qX1741VkHjTCPem1u4clg==', N'a8c7719d-d976-40c3-99fb-7771c2f9bffe', N'Dinesh', N'Venkatachalapathy', N'vdinesh76@gmail.com', N'ApplicationUser', N'123456', 2, N'Shefa Agricare Technologies(Hubli)', 0, NULL)
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'd3f16934-2480-47d6-b579-3575267110c9', N'sadmin', N'AFT5rrUhVLBx2cxosP6gYIEWlVnRyxthKff/g/nQ45NTkB0jFJyWMEV4ffcEwUPIBw==', N'37e10304-6b8c-41dc-9e07-419aff9ce846', N's', N's', N'sales@fusiontec.com', N'ApplicationUser', N'123456', 1, N'Main Stores', 8, N'Main Stores')
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'daebe411-6cf0-4770-ae44-d0d4af4e501b', N'admin', N'AFQkyDftHbt5MB/A6AC94ETjygnRxJIzPvDwvCzQ5VzwneAu9MRl6DPqWJ8BNAFcZQ==', N'a6b0b553-4f5d-4ecb-aa6d-12ca96078e51', N'.', N'.', N'.', N'ApplicationUser', N'123456', 1, N'Main Stores', 35, N'Admin')
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [FirstName], [LastName], [Email], [Discriminator], [NPassword], [BrnchId], [UBrnchName], [DBrnchId], [DeptName]) VALUES (N'e5302e13-2700-44b0-bf7c-98e20542846c', N'akola', N'AKx3AionID8n7J5osTosPH8Z5Sc6GupeVB53c5kT1YxMLCiSG3RJMJUGqUaa7Twvyg==', N'bc4eb71e-5bf5-46e8-bd88-d24b6dcc1ed0', N'-', N'-', N'akola@shefa.co.in', N'ApplicationUser', N'654123', 5, N'SHEFA AGRICARE TECHNOLOGIES PRIVATE LIMITED(AKOLA)', 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Groups] ON 
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (1, N'SuperAdmin')
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (2, N'Admin')
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (3, N'Manager')
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (4, N'Users')
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (10, N'Branch Manager')
GO
INSERT [dbo].[Groups] ([Id], [Name]) VALUES (11, N'Branch User')
GO
SET IDENTITY_INSERT [dbo].[Groups] OFF
GO
SET IDENTITY_INSERT [dbo].[MenuRoleMaster] ON 
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024355, N'Company Master', N'Index', N'CompanyMaster', N'sadmin', 1, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024356, N'Home', N'Index', N'Home', N'sadmin', 1, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024357, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'sadmin', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024358, N'Branch Master', N'Index', N'BranchMaster', N'sadmin', 2, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024359, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'sadmin', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024360, N'Purchase Order', N'Index', N'PurchaseOrderRpt', N'sadmin', 2, 3, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024361, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'sadmin', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024362, N'Cost Factor', N'Index', N'CostFactorMaster', N'sadmin', 3, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024363, N'Client Order', N'Index', N'BranchClientOrder', N'sadmin', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024364, N'Department Master', N'Index', N'DepartmentMaster', N'sadmin', 4, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024365, N'Purchase Invoice', N'Index', N'PurchaseInvoiceRpt', N'sadmin', 4, 3, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024366, N'MaterialGroup Master', N'Index', N'MaterialGroupMaster', N'sadmin', 5, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024367, N'Material Master', N'Index', N'MaterialMaster', N'sadmin', 6, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024368, N'RateCardMaster', N'Index', N'RateCardMaster', N'sadmin', 7, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024369, N'Designation', N'Index', N'DesignationMaster', N'sadmin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024370, N'GatePass', N'Index', N'DesignationMaster', N'sadmin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024371, N'SupplierMaster', N'Index', N'SupplierMaster', N'sadmin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024372, N'Unit', N'Index', N'UnitMaster', N'sadmin', 9, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024373, N'HSNCode', N'Index', N'HSNCodeMaster', N'sadmin', 10, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024374, N'State Master', N'Index', N'StateMaster', N'sadmin', 10, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024375, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'sadmin', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024376, N'Location Master', N'Index', N'LocationMaster', N'sadmin', 11, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024377, N'Customer Master', N'Index', N'CustomerMaster', N'sadmin', 14, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024378, N'Employee Master', N'Index', N'EmployeeMaster', N'sadmin', 15, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024379, N'MainStoreStockView', N'Index', N'MainStoreStockView', N'sadmin', 15, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024380, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'sadmin', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024381, N'HO Sales Invoice', N'Index', N'HOSalesInvoice', N'sadmin', 23, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024382, N'Branch Stock', N'Index', N'BranchStoreStockView', N'sadmin', 23, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024383, N'Purchase Order', N'Index', N'PurchaseOrder', N'sadmin', 24, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024384, N'GoodsReceiptNote', N'Index', N'GoodsReceiptNote', N'sadmin', 25, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024385, N'Purchase Invoice', N'Index', N'PurchaseInvoice', N'sadmin', 26, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024386, N'HO Opening', N'Index', N'MainStoreOpening', N'sadmin', 32, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024387, N'PurchaseIndent', N'Index', N'MainStoreOpening', N'sadmin', 32, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024388, N'Client Order', N'Index', N'BranchClientOrderRpt', N'sadmin', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1024389, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'sadmin', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032946, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'suresh', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032947, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'suresh', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032948, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'suresh', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032949, N'Client Order', N'Index', N'BranchClientOrder', N'suresh', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032950, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'suresh', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032951, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'suresh', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032952, N'Branch Transfer', N'Index', N'BranchTransfer', N'suresh', 22, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032953, N'Sales Return', N'Index', N'BranchSalesInvoiceReturn', N'suresh', 23, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032954, N'Branch Stock', N'Index', N'BranchStoreStockView', N'suresh', 24, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032955, N'Client Order', N'Index', N'BranchClientOrderRpt', N'suresh', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1032956, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'suresh', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175979, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'akola', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175980, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'akola', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175981, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'akola', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175982, N'Dealer Target Plan Vs. Actuals', N'Index', N'DealerTargetPlan', N'akola', 2, 8, N'fa fa-bullseye')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175983, N'Client Order', N'Index', N'BranchClientOrder', N'akola', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175984, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'akola', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175985, N'E-Invoice', N'Index', N'BranchEInvoice', N'akola', 21, 7, N'fa fa-fw fa-bullhorn')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175986, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'akola', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175987, N'Branch Transfer', N'Index', N'BranchTransfer', N'akola', 22, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175988, N'Sales Return', N'Index', N'BranchSalesInvoiceReturn', N'akola', 23, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175989, N'Branch Stock', N'Index', N'BranchStoreStockView', N'akola', 24, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175990, N'Client Order', N'Index', N'BranchClientOrderRpt', N'akola', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1175991, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'akola', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183540, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'csr', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183541, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'csr', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183542, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'csr', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183543, N'Dealer Target Plan Vs. Actuals', N'Index', N'DealerTargetPlan', N'csr', 2, 8, N'fa fa-bullseye')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183544, N'Client Order', N'Index', N'BranchClientOrder', N'csr', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183545, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'csr', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183546, N'E-Invoice', N'Index', N'BranchEInvoice', N'csr', 21, 7, N'fa fa-fw fa-bullhorn')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183547, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'csr', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183548, N'Branch Transfer', N'Index', N'BranchTransfer', N'csr', 22, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183549, N'Sales Return', N'Index', N'BranchSalesInvoiceReturn', N'csr', 23, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183550, N'Branch Stock', N'Index', N'BranchStoreStockView', N'csr', 24, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183551, N'Client Order', N'Index', N'BranchClientOrderRpt', N'csr', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1183552, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'csr', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184599, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'dinesh', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184600, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'dinesh', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184601, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'dinesh', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184602, N'Dealer Target Plan Vs. Actuals', N'Index', N'DealerTargetPlan', N'dinesh', 2, 8, N'fa fa-bullseye')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184603, N'Client Order', N'Index', N'BranchClientOrder', N'dinesh', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184604, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'dinesh', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184605, N'E-Invoice', N'Index', N'BranchEInvoice', N'dinesh', 21, 7, N'fa fa-fw fa-bullhorn')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184606, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'dinesh', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184607, N'Branch Transfer', N'Index', N'BranchTransfer', N'dinesh', 22, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184608, N'Sales Return', N'Index', N'BranchSalesInvoiceReturn', N'dinesh', 23, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184609, N'Branch Stock', N'Index', N'BranchStoreStockView', N'dinesh', 24, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184610, N'Client Order', N'Index', N'BranchClientOrderRpt', N'dinesh', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184611, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'dinesh', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184812, N'Company Master', N'Index', N'CompanyMaster', N'admin', 1, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184813, N'Home', N'Index', N'Home', N'admin', 1, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184814, N'BranchStore Opening', N'Index', N'BranchStoreOpening', N'admin', 1, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184815, N'Branch Master', N'Index', N'BranchMaster', N'admin', 2, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184816, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'admin', 2, 3, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184817, N'Purchase Order', N'Index', N'PurchaseOrderRpt', N'admin', 2, 3, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184818, N'Purchase Invoice', N'Index', N'BranchPurchaseInvoice', N'admin', 2, 7, N'fa fa-fw fa-automobile')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184819, N'Dealer Target Plan Vs. Actuals', N'Index', N'DealerTargetPlan', N'admin', 2, 8, N'fa fa-bullseye')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184820, N'Cost Factor', N'Index', N'CostFactorMaster', N'admin', 3, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184821, N'Client Order', N'Index', N'BranchClientOrder', N'admin', 3, 7, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184822, N'Department Master', N'Index', N'DepartmentMaster', N'admin', 4, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184823, N'Category Master', N'Index', N'CategoryMaster', N'admin', 4, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184824, N'Purchase Invoice', N'Index', N'PurchaseInvoiceRpt', N'admin', 4, 3, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184825, N'MaterialGroup Master', N'Index', N'MaterialGroupMaster', N'admin', 5, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184826, N'Material Master', N'Index', N'MaterialMaster', N'admin', 6, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184827, N'RateCardMaster', N'Index', N'RateCardMaster', N'admin', 7, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184828, N'Designation', N'Index', N'DesignationMaster', N'admin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184829, N'GatePass', N'Index', N'DesignationMaster', N'admin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184830, N'SupplierMaster', N'Index', N'SupplierMaster', N'admin', 8, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184831, N'Unit', N'Index', N'UnitMaster', N'admin', 9, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184832, N'HSNCode', N'Index', N'HSNCodeMaster', N'admin', 10, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184833, N'State Master', N'Index', N'StateMaster', N'admin', 10, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184834, N'Branch Store Stock', N'Index', N'BranchStoreStockRpt', N'admin', 10, 3, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184835, N'Location Master', N'Index', N'LocationMaster', N'admin', 11, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184836, N'Customer Master', N'Index', N'CustomerMaster', N'admin', 14, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184837, N'Employee Master', N'Index', N'EmployeeMaster', N'admin', 15, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184838, N'MainStoreStockView', N'Index', N'MainStoreStockView', N'admin', 15, 1, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184839, N'E-Invoice', N'Index', N'SalesEInvoice', N'admin', 21, 7, N'fa fa-fw fa-bullhorn')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184840, N'Sales Invoice', N'Index', N'BranchSalesInvoice', N'admin', 21, 7, N'fa fa-fw fa-stumbleupon')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184841, N'Branch Transfer', N'Index', N'BranchTransfer', N'admin', 22, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184842, N'HO Sales Invoice', N'Index', N'HOSalesInvoice', N'admin', 23, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184843, N'Sales Return', N'Index', N'BranchSalesInvoiceReturn', N'admin', 23, 7, N'fa fa-fw fa-cog')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184844, N'Purchase Order', N'Index', N'PurchaseOrder', N'admin', 24, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184845, N'Branch Stock', N'Index', N'BranchStoreStockView', N'admin', 24, 7, N'fa fa-fw fa-bitbucket')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184846, N'GoodsReceiptNote', N'Index', N'GoodsReceiptNote', N'admin', 25, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184847, N'Purchase Invoice', N'Index', N'PurchaseInvoice', N'admin', 26, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184848, N'HO Opening', N'Index', N'MainStoreOpening', N'admin', 32, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184849, N'PurchaseIndent', N'Index', N'MainStoreOpening', N'admin', 32, 2, NULL)
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184850, N'Client Order', N'Index', N'BranchClientOrderRpt', N'admin', 33, 33, N'fa fa-fw fa-briefcase')
GO
INSERT [dbo].[MenuRoleMaster] ([MenuId], [LinkText], [ActionName], [ControllerName], [Roles], [MenuGId], [MenuGIndex], [ImageClassName]) VALUES (1184851, N'Sales Invoice', N'Index', N'BranchSalesInvoiceRpt', N'admin', 34, 33, N'fa fa-fw fa-stumbleupon')
GO
SET IDENTITY_INSERT [dbo].[MenuRoleMaster] OFF
GO
ALTER TABLE [dbo].[AspNetRoles] ADD  CONSTRAINT [DF_AspNetRoles_SDPTID]  DEFAULT ((0)) FOR [SDPTID]
GO
ALTER TABLE [dbo].[AspNetRoles] ADD  CONSTRAINT [DF_AspNetRoles_RImageClassName]  DEFAULT (NULL) FOR [RImageClassName]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_BrnchId_1]  DEFAULT ((0)) FOR [BrnchId]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  CONSTRAINT [DF_AspNetUsers_BrnchId1]  DEFAULT ((0)) FOR [DBrnchId]
GO
ALTER TABLE [dbo].[MenuRoleMaster] ADD  CONSTRAINT [DF_MenuRoleMaster_ImageClassName]  DEFAULT (NULL) FOR [ImageClassName]
GO
ALTER TABLE [dbo].[ApplicationRoleGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationRoleGroups_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicationRoleGroups] CHECK CONSTRAINT [FK_dbo.ApplicationRoleGroups_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[ApplicationRoleGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationRoleGroups_dbo.Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicationRoleGroups] CHECK CONSTRAINT [FK_dbo.ApplicationRoleGroups_dbo.Groups_GroupId]
GO
ALTER TABLE [dbo].[ApplicationUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicationUserGroups] CHECK CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[ApplicationUserGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicationUserGroups] CHECK CONSTRAINT [FK_dbo.ApplicationUserGroups_dbo.Groups_GroupId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id] FOREIGN KEY([User_Id])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
