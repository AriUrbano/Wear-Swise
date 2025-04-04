USE [master]
GO
/****** Object:  Database [Info360]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[categorias]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[detalles_pedido]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[envios]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[metodos_pago]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[pagos]    Script Date: 3/4/2025 21:41:38 ******/
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
/****** Object:  Table [dbo].[pedidos]    Script Date: 3/4/2025 21:41:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pedidos](
	[id_pedido] [int] IDENTITY(1,1) NOT NULL,
	[id_usuario] [int] NOT NULL,
	[fecha_pedido] [datetime] NULL,
	[estado] [varchar](10) NOT NULL,
	[total] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_pedido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[productos]    Script Date: 3/4/2025 21:41:38 ******/
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
	[id_vendedor] [int] NULL,
	[fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[usuarios]    Script Date: 3/4/2025 21:41:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__usuarios__5B8A0682B0FD7CC9] UNIQUE NONCLUSTERED 
(
	[correo_electronico] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[envios] ADD  DEFAULT (getdate()) FOR [fecha_envio]
GO
ALTER TABLE [dbo].[pagos] ADD  DEFAULT (getdate()) FOR [fecha_pago]
GO
ALTER TABLE [dbo].[pedidos] ADD  DEFAULT (getdate()) FOR [fecha_pedido]
GO
ALTER TABLE [dbo].[productos] ADD  DEFAULT (getdate()) FOR [fecha_creacion]
GO
ALTER TABLE [dbo].[detalles_pedido]  WITH CHECK ADD FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[detalles_pedido]  WITH CHECK ADD FOREIGN KEY([id_producto])
REFERENCES [dbo].[productos] ([id_producto])
GO
ALTER TABLE [dbo].[envios]  WITH CHECK ADD FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[pagos]  WITH CHECK ADD FOREIGN KEY([id_metodo_pago])
REFERENCES [dbo].[metodos_pago] ([id_metodo_pago])
GO
ALTER TABLE [dbo].[pagos]  WITH CHECK ADD FOREIGN KEY([id_pedido])
REFERENCES [dbo].[pedidos] ([id_pedido])
GO
ALTER TABLE [dbo].[pedidos]  WITH CHECK ADD  CONSTRAINT [FK__pedidos__id_usua__59063A47] FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[pedidos] CHECK CONSTRAINT [FK__pedidos__id_usua__59063A47]
GO
ALTER TABLE [dbo].[productos]  WITH CHECK ADD FOREIGN KEY([id_categoria])
REFERENCES [dbo].[categorias] ([id_categoria])
GO
ALTER TABLE [dbo].[productos]  WITH CHECK ADD  CONSTRAINT [FK__productos__id_ve__5AEE82B9] FOREIGN KEY([id_vendedor])
REFERENCES [dbo].[usuarios] ([id_usuario])
GO
ALTER TABLE [dbo].[productos] CHECK CONSTRAINT [FK__productos__id_ve__5AEE82B9]
GO
ALTER TABLE [dbo].[envios]  WITH CHECK ADD CHECK  (([estado]='entregado' OR [estado]='en tránsito' OR [estado]='pendiente'))
GO
ALTER TABLE [dbo].[pedidos]  WITH CHECK ADD CHECK  (([estado]='cancelado' OR [estado]='entregado' OR [estado]='enviado' OR [estado]='pendiente'))
GO
USE [master]
GO
ALTER DATABASE [Info360] SET  READ_WRITE 
GO
