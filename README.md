## PRYECTO BACKEND ##
Este proyecto .NET es una API construida en .NET 8 con patron CQRS y multi-capas. Funcionalidades:
- inserta un producto, obtiene lista completa de productos en base a paginación.
- inserta un producto nuevo. El campo de "descuento" lo obtiene de una API externa de mockapi.io
- obtiene un producto por su ID.
- actualiza un producto.


**Principales dependencias:**
- [.NET Core 8]
- [MediatR]
- [Carter]
- [ORM Dapper]
- [FluentValidation]
- [Automapper]
- [Swashbuckle.AspNetCore]
- [NLOG]

## Persistencia: ##
- Los datos de los productos quedarán persistidos en **SQL Server Local** (puede ser SQL Server Express, SQL Server Standar o SQL Server Enterprise). Si no tiene SQL Server instalado localmente la solución permite levantar SQL Server en **DOCKER**.

## Configuración de la Base de Datos local ##

1. Para depuración con SQL local debe contar con SQL Server Express, SQL Server Standar o SQL Server Enterprise. Debe abrir SQL Management Studio o Azure Data Studio y ejecutar los pasos siguientes:
- Crear manualmente una BD llamada **BD_Products** vacía.
- Ejecutar el script **1_Create_Tables.sql** ubicado en la carpeta **BackEndProducts.Sql\Scripts-BD**. Dicho script debe ejecutarlo en la BD nueva anteriormente creada. Este script creará la tabla **dbo.Products**.
 

2. Luego debe ir al archivo **appsettings.json** del proyecto **BackEndProducts.Api** y deberá editar la **cadenaConexion**, específicamente los valores marcados con **XXXXXX**:

- Server Host
- Server Port (Si utiliza el puerto TCP 1433, puede evitar este parámetro o definir 1433 directamente.)
- User
- Password

"cadenaConexion": "Server=**XXXXXX**,**XXXXXX**;Initial Catalog=BD_Products;User ID=**XXXXXX**;Password=**XXXXXX**;TrustServerCertificate=true"


## Configuración de SQL Server en Docker ##

Puede ejecutar la solución en contenedores Docker, debe contar con un servcio de Docker o Docker Desktop instalado en su equipo. Luego debe ir a la carpeta donde se encuentra la solución **BackEndProducts.sln**, y abrir una ventana cmd, y ejecutar el comando  **docker compose up**. Este comando descargará las imagenes de SQL Server 2019 y de la API, y luego creará y levantará dos contenedores: 

- Contenedor de API se iniciará en el puerto **13000**. Puede validar abriendo un browser e ir a la URL **http://localhost:13000/swagger/index.html** debería ver la lista de métodos expuestos.
- Contenedor de SQL Server se iniciará en el puerto **9001**. Puede validar abriendo **SQL Management Studio** o **Azure Data Studio**, y conectarse a **localhost:9001** y con usuario **sa** y password **Password1*** y debería ver el servidor sin bases de datos de usuario. Una vez ahí conetcado al servidor deberá: 
- Crear una ueva Bade de datos: Si no a creado una Base de Datos nueva llamada **BD_Products**, favor creela manualmente. 
- Ejecutar el script **1_Create_Tables.sql** ubicado en la carpeta **BackEndProducts.Sql\Scripts-BD\**. Dicho script debe ejecutarlo en la BD nueva anteriormente creada. Este script creará la tabla **dbo.Products**. 

## Ejecución de API apuntando a BD local ##
Puede ejecutar la api en forma local. Pasos:
- Abrir la solución **BackEndProducts.sln** en **Visual Studio 2022**
- Asigar el proyecto **BackEndProducts.Api** como proyecto por defecto de inicio.
- Ejeuctar F5 para iniciar la depuración, la que se iniciará en el puerto **9000*, por lo que puede abrir el browser en la URL **http://localhost:13000/swagger/index.html** y podrá ver la lista de métodos expuestos.
- **Nota**: Para esta depuración local debe haber configurado previamente la cadena de conexión **cadenaConexion** del archivo **appsettings.json** ubicado en el proyecto **BackEndProducts.Api**, . y apuntar a su la base de datos local (SQL en puerto **1433** por defecto) o al la BD de Docker, puerto **9001**.


## Ejecución de API apuntando a contenedor Docker ##
Puede ejecutar la api apuntando a contenedor Docker. Pasos:
- Ir a la carpeta donde se encuentra la solución **BackEndProducts.sln**, y abrir una ventana cmd, y ejecutar el comando **docker compose up**. Este comando descargará las imagenes de SQL Server 2019 y de la API, y luego creará y levantará dos contenedores: 

-Contenedor de API se iniciará en el puerto **13000**. Para validar: abrir al browser en **http://localhost:13000/swagger/index.html** debería ver la lista de métodos expuestos.
-Contenedor de SQL Server se iniciará en el puerto **9001**.

- FIN -