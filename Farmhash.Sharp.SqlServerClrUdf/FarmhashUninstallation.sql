SET NOCOUNT ON ;
GO

-- !! MODIFY TO SUIT YOUR TEST ENVIRONMENT !!
USE MinhashTest
GO

-------------------------------------------------------------------------------------------------------------------------------

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[StringFarmhash64]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[StringFarmhash64]
GO

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[BinaryFarmhash64]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[BinaryFarmhash64]
GO

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Farmhash64]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[Farmhash64]
GO

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[StringFarmhash32]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[StringFarmhash32]
GO

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[BinaryFarmhash32]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[BinaryFarmhash32]
GO

IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Farmhash32]')
                    AND type = N'FS' ) 
    DROP FUNCTION [dbo].[Farmhash32]
GO

IF EXISTS ( SELECT  *
            FROM    sys.assemblies asms
            WHERE   asms.name = N'Farmhash.Sharp.SqlServerClrUdf'
                    AND is_user_defined = 1 ) 
    DROP ASSEMBLY [Farmhash.Sharp.SqlServerClrUdf]
GO

IF EXISTS ( SELECT  *
            FROM    sys.assemblies asms
            WHERE   asms.name = N'Farmhash.Sharp.Safe'
                    AND is_user_defined = 1 ) 
    DROP ASSEMBLY [Farmhash.Sharp.Safe]
GO

-------------------------------------------------------------------------------------------------------------------------------
