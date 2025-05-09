USE [master]
GO
/****** Object:  Database [Info360]    Script Date: 1/5/2025 20:19:06 ******/
CREATE DATABASE [Info360]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Info360', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Info360.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Info360_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Info360_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Info360] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Info360].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Info360] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Info360] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Info360] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Info360] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Info360] SET ARITHABORT OFF 
GO
ALTER DATABASE [Info360] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Info360] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Info360] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Info360] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Info360] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Info360] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Info360] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Info360] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Info360] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Info360] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Info360] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Info360] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Info360] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Info360] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Info360] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Info360] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Info360] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Info360] SET RECOVERY FULL 
GO
ALTER DATABASE [Info360] SET  MULTI_USER 
GO
ALTER DATABASE [Info360] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Info360] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Info360] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Info360] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Info360] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Info360] SET QUERY_STORE = OFF
GO
USE [Info360]
GO
/****** Object:  Table [dbo].[categorias]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categorias](
	[id_categoria] [int] IDENTITY(1,1) NOT NULL,
	[nombre_categoria] [varchar](50) NOT NULL,
	[descripcion] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[detalles_pedido]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[detalles_pedido](
	[id_detalle_pedido] [int] IDENTITY(1,1) NOT NULL,
	[id_pedido] [int] NOT NULL,
	[id_producto] [int] NOT NULL,
	[cantidad] [int] NOT NULL,
	[precio] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_detalle_pedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[envios]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[envios](
	[id_envio] [int] IDENTITY(1,1) NOT NULL,
	[id_pedido] [int] NOT NULL,
	[fecha_envio] [datetime] NULL,
	[numero_seguimiento] [varchar](50) NULL,
	[estado] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_envio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[metodos_pago]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[metodos_pago](
	[id_metodo_pago] [int] IDENTITY(1,1) NOT NULL,
	[nombre_metodo] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_metodo_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pagos]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pagos](
	[id_pago] [int] IDENTITY(1,1) NOT NULL,
	[id_pedido] [int] NOT NULL,
	[id_metodo_pago] [int] NOT NULL,
	[fecha_pago] [datetime] NULL,
	[cantidad] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[pedidos]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pedidos](
	[id_pedido] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [int] NOT NULL,
	[fecha_pedido] [datetime] NULL,
	[estado] [varchar](10) NULL,
	[total] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK__pedidos__6FF01489695C1B1C] PRIMARY KEY CLUSTERED 
(
	[id_pedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[productos]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[productos](
	[id_producto] [int] IDENTITY(1,1) NOT NULL,
	[nombre_producto] [varchar](100) NOT NULL,
	[descripcion] [text] NULL,
	[precio] [decimal](10, 2) NOT NULL,
	[stock] [int] NOT NULL,
	[id_categoria] [int] NULL,
	[fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usuarios]    Script Date: 1/5/2025 20:19:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[usuarios](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[nombre_usuario] [varchar](50) NOT NULL,
	[correo_electronico] [varchar](100) NOT NULL,
	[contrasena] [varchar](255) NOT NULL,
 CONSTRAINT [PK__usuarios__4E3E04AD73C8C643] PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[categorias] ON 

INSERT [dbo].[categorias] ([id_categoria], [nombre_categoria], [descripcion]) VALUES (1, N'Ropa', N'Prendas de vestir')
SET IDENTITY_INSERT [dbo].[categorias] OFF
GO
SET IDENTITY_INSERT [dbo].[detalles_pedido] ON 

INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1, 1, 2, 63, CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (2, 1, 3, 2, CAST(59.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1002, 2, 2, 1, CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1003, 3, 2, 1, CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1004, 4, 3, 112, CAST(59.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1005, 5, 2, 12, CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[detalles_pedido] ([id_detalle_pedido], [id_pedido], [id_producto], [cantidad], [precio]) VALUES (1006, 6, 2, 12, CAST(19.99 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[detalles_pedido] OFF
GO
SET IDENTITY_INSERT [dbo].[pedidos] ON 

INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (1, 2, CAST(N'2025-04-27T15:54:14.557' AS DateTime), N'entregado', CAST(1379.35 AS Decimal(10, 2)))
INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (2, 3, CAST(N'2025-04-27T22:11:25.673' AS DateTime), N'entregado', CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (3, 2, CAST(N'2025-05-01T20:15:10.010' AS DateTime), N'entregado', CAST(19.99 AS Decimal(10, 2)))
INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (4, 2, CAST(N'2025-05-01T20:15:41.333' AS DateTime), N'entregado', CAST(6718.88 AS Decimal(10, 2)))
INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (5, 3, CAST(N'2025-05-01T20:16:04.713' AS DateTime), N'entregado', CAST(239.88 AS Decimal(10, 2)))
INSERT [dbo].[pedidos] ([id_pedido], [id_usuario], [fecha_pedido], [estado], [total]) VALUES (6, 2, CAST(N'2025-05-01T20:17:56.510' AS DateTime), N'pendiente', CAST(0.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[pedidos] OFF
GO
SET IDENTITY_INSERT [dbo].[productos] ON 

INSERT [dbo].[productos] ([id_producto], [nombre_producto], [descripcion], [precio], [stock], [id_categoria], [fecha_creacion]) VALUES (2, N'Camiseta Blanca Básica', N'Camiseta 100% algodón, corte clásico', CAST(19.99 AS Decimal(10, 2)), 23, 1, CAST(N'2025-04-27T15:53:54.980' AS DateTime))
INSERT [dbo].[productos] ([id_producto], [nombre_producto], [descripcion], [precio], [stock], [id_categoria], [fecha_creacion]) VALUES (3, N'Jeans Azul Clásico', N'Pantalón de mezclilla corte regular', CAST(59.99 AS Decimal(10, 2)), -39, 1, CAST(N'2025-04-27T15:53:54.980' AS DateTime))
INSERT [dbo].[productos] ([id_producto], [nombre_producto], [descripcion], [precio], [stock], [id_categoria], [fecha_creacion]) VALUES (4, N'Sudadera con Capucha Negra', N'Sudadera abrigada con bolsillo canguro', CAST(39.99 AS Decimal(10, 2)), 60, 1, CAST(N'2025-04-27T15:53:54.980' AS DateTime))
INSERT [dbo].[productos] ([id_producto], [nombre_producto], [descripcion], [precio], [stock], [id_categoria], [fecha_creacion]) VALUES (5, N'Zapatillas Deportivas', N'Calzado cómodo para uso diario', CAST(49.99 AS Decimal(10, 2)), 50, 1, CAST(N'2025-04-27T15:53:54.980' AS DateTime))
SET IDENTITY_INSERT [dbo].[productos] OFF
GO
SET IDENTITY_INSERT [dbo].[usuarios] ON 

INSERT [dbo].[usuarios] ([id_usuario], [nombre_usuario], [correo_electronico], [contrasena]) VALUES (2, N'Ari', N'ariurb11@gmail.com', N'123456')
INSERT [dbo].[usuarios] ([id_usuario], [nombre_usuario], [correo_electronico], [contrasena]) VALUES (3, N'polshu', N'Pulshu@gmail.com', N'123456')
INSERT [dbo].[usuarios] ([id_usuario], [nombre_usuario], [correo_electronico], [contrasena]) VALUES (1003, N'Ariurb', N'Menace@gmail.com', N'123456')
SET IDENTITY_INSERT [dbo].[usuarios] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__usuarios__5B8A0682B0FD7CC9]    Script Date: 1/5/2025 20:19:07 ******/
ALTER TABLE [dbo].[usuarios] ADD  CONSTRAINT [UQ__usuarios__5B8A0682B0FD7CC9] UNIQUE NONCLUSTERED 
(
	[correo_electronico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[envios] ADD  DEFAULT (getdate()) FOR [fecha_envio]
GO
ALTER TABLE [dbo].[pagos] ADD  DEFAULT (getdate()) FOR [fecha_pago]
GO
ALTER TABLE [dbo].[pedidos] ADD  CONSTRAINT [DF__pedidos__fecha_p__59FA5E80]  DEFAULT (getdate()) FOR [fecha_pedido]
GO
ALTER TABLE [dbo].[productos] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[detalles_pedido]  WITH CHECK ADD  CONSTRAINT [FK__detalles___id_pe__5BE2A6F2] FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[detalles_pedido] CHECK CONSTRAINT [FK__detalles___id_pe__5BE2A6F2]
GO
ALTER TABLE [dbo].[detalles_pedido]  WITH CHECK ADD FOREIGN KEY([id_producto])
REFERENCES [dbo].[productos] ([id_producto])
GO
ALTER TABLE [dbo].[envios]  WITH CHECK ADD  CONSTRAINT [FK__envios__id_pedid__5DCAEF64] FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[envios] CHECK CONSTRAINT [FK__envios__id_pedid__5DCAEF64]
GO
ALTER TABLE [dbo].[pagos]  WITH CHECK ADD FOREIGN KEY([id_metodo_pago])
REFERENCES [dbo].[metodos_pago] ([id_metodo_pago])
GO
ALTER TABLE [dbo].[pagos]  WITH CHECK ADD  CONSTRAINT [FK__pagos__id_pedido__5FB337D6] FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[pagos] CHECK CONSTRAINT [FK__pagos__id_pedido__5FB337D6]
GO
ALTER TABLE [dbo].[pedidos]  WITH CHECK ADD  CONSTRAINT [FK__pedidos__id_usua__59063A47] FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[pedidos] CHECK CONSTRAINT [FK__pedidos__id_usua__59063A47]
GO
ALTER TABLE [dbo].[productos]  WITH CHECK ADD FOREIGN KEY([id_categoria])
REFERENCES [dbo].[categorias] ([id_categoria])
GO
ALTER TABLE [dbo].[envios]  WITH CHECK ADD CHECK  (([estado]='entregado' OR [estado]='en tránsito' OR [estado]='pendiente'))
GO
ALTER TABLE [dbo].[pedidos]  WITH CHECK ADD  CONSTRAINT [CK__pedidos__estado__6477ECF3] CHECK  (([estado]='cancelado' OR [estado]='entregado' OR [estado]='enviado' OR [estado]='pendiente'))
GO
ALTER TABLE [dbo].[pedidos] CHECK CONSTRAINT [CK__pedidos__estado__6477ECF3]
GO
USE [master]
GO
ALTER DATABASE [Info360] SET  READ_WRITE 
GO
