Imports System.Drawing
Imports System.Collections.Generic
Imports _3DAPI
Imports _3DLib_OpenGL
Imports System.IO
Imports _3DAPI.Physics
Module Module1

    Sub Main()
        GlTestApp.MainClass.Main()
    End Sub

End Module

Namespace GlTestApp
    Class MainClass
        Shared collisiontester As PhysicalObject
        Shared physicsworld As New World()
        Shared flip As Boolean = False
        Shared rtex As Texture2D
        Public Shared Sub Main()

            Try

                ' throw new Exception();
                renderer = New DirectXLib.DirectEngine()
                flip = True
            Catch generatedExceptionName As Exception
                renderer = New GLRenderer()
                flip = False
            End Try
            renderer.cameraPosition.Z = -5

            Dim myder As Shader = renderer.createBasicShader()

            'Uncomment for AABB collisions

            ' physicsworld.testtype = CollisionTestType.AABB;
            myder.Draw()

            Dim mmap As New Bitmap("pic.jpg")
            Dim mtex As Texture2D = renderer.createTextureFromBitmap(mmap)
            mtex.Draw()
            rtex = renderer.createTexture(512, 512)

            rtex.Draw()
            Dim meshes As Mesh() = Primitives.LoadMesh("playercube.obj", flip)

            collisiontester = New PhysicalObject(meshes(0).meshverts, 5, CollisionType.Dynamic, physicsworld)
            collisiontester.Position = New Vector3D(-5, 0, 0)

            mainmesh = meshes(0)
            For Each mesh As Mesh In meshes
                Dim tbuff As VertexBuffer = renderer.CreateVertexBuffer(mesh.meshverts, mesh.meshtexas, mesh.meshnorms)
                rotatingbuffer = tbuff
                If mesh.bitmap IsNot Nothing Then
                    Console.WriteLine("BITMAP RENDER")
                    Dim tt As Texture2D = renderer.createTextureFromBitmap(mesh.bitmap)
                    tt.Draw()
                End If
                tbuff.Draw()
            Next
            mtex.Draw()
            Dim cube As Mesh = Primitives.LoadMesh("playercube.obj", flip)(0)

            theobject = New PhysicalObject(cube.meshverts, 9, CollisionType.Dynamic, physicsworld)


            collisiontester.ownedVBO = rotatingbuffer
            theobject.ownedVBO = renderer.CreateVertexBuffer(cube.meshverts, cube.meshtexas, cube.meshnorms)

            theobject.ownedVBO.Draw()



            physicsworld.physicalobjects.Add(theobject)
            physicsworld.physicalobjects.Add(collisiontester)

            Dim anothercube As Mesh = Primitives.LoadMesh("playercube.obj", flip)(0)
            Dim mobject As New PhysicalObject(TryCast(anothercube.meshverts.Clone(), Vector3D()), 1, CollisionType.Dynamic, physicsworld)
            mobject.ownedVBO = renderer.CreateVertexBuffer(anothercube.meshverts, anothercube.meshtexas, anothercube.meshnorms)
            physicsworld.physicalobjects.Add(mobject)
            mobject.ownedVBO.Draw()
            mobject.Position = New Vector3D(30, 0, 0)
            'Set physics properties

            theobject.Weight = 1

            collisiontester.Weight = 1

            'collisiontester.Velocity = new Vector3D(.05f, 0, 0);
            theobject.Position = New Vector3D(15, 0, 0)
            theobject.IsCube = True
            theobject.Weight = 9999999
            mobject.IsCube = True

            'End physics properties

            physicsworld.Start()

            AddHandler physicsworld.physicsUpdateFrame, AddressOf physicsworld_physicsUpdateFrame

            Dim mthread As New System.Threading.Thread(AddressOf thetar)
            mthread.Start()
            AddHandler renderer.defaultKeyboard.onKeyDown, AddressOf defaultKeyboard_onKeyDown
            AddHandler renderer.defaultKeyboard.onKeyUp, AddressOf defaultKeyboard_onKeyUp
            AddHandler renderer.defaultMouse.onMouseMove, AddressOf defaultMouse_onMouseMove
            AddHandler renderer.defaultMouse.onMouseDown, AddressOf defaultMouse_onMouseDown
        End Sub

        Private Shared Sub physicsworld_physicsUpdateFrame()

            renderer.SetRenderTarget(rtex, collisiontester.Position, New Vector3D(180, 0, 0))
        End Sub

        Private Shared Sub defaultMouse_onMouseDown(btn As MouseButton, x As Integer, y As Integer)
            SyncLock physicsworld.physicalobjects
                Dim tmesh As Mesh = Primitives.LoadMesh("playercube.obj", flip)(0)
                Dim mobject As New PhysicalObject(tmesh.meshverts, 1, CollisionType.Dynamic, physicsworld)
                mobject.IsCube = True
                mobject.ownedVBO = renderer.CreateVertexBuffer(tmesh.meshverts, tmesh.meshtexas, tmesh.meshnorms)
                mobject.ownedVBO.Draw()

                physicsworld.physicalobjects.Add(mobject)
                mobject.Position = renderer.cameraPosition
                If btn = MouseButton.Left Then


                    mobject.Velocity = Vector3D.ComputeRotation(renderer.worldRotation)
                Else

                    mobject.Weight = 3.0F

                End If
            End SyncLock
        End Sub
        Shared prevval As New Vector2D(0, 0)
        Shared move As Boolean = False
        Private Shared Sub defaultMouse_onMouseMove(btn As MouseButton, x As Integer, y As Integer)
            If prevval.X = 0 Then
                prevval.X = x
                prevval.Y = y
            End If
            If move Then
                renderer.worldRotation.Y += (prevval.X - x) / 10
                renderer.worldRotation.X += (prevval.Y - y) / 10

                'Windows Phone doesn't support setting the cursor position
#If Not PHONE Then
                System.Windows.Forms.Cursor.Position = New Point(CInt(prevval.X), CInt(prevval.Y))
#End If

                move = False
            Else
                move = True
            End If
        End Sub
        Shared ctrl As Boolean = False
        Private Shared Sub defaultKeyboard_onKeyUp(KeyName As String)
            If KeyName.ToLower().Contains("control") Then
                ctrl = False
            End If
            If KeyName.ToLower().Contains("up") Then

                up = False
            End If
            If KeyName.ToLower().Contains("down") Then
                down = False
            End If
            If KeyName.ToLower().Contains("left") Then
                left = False
            End If
            If KeyName.ToLower().Contains("right") Then
                right = False
            End If

        End Sub

        Private Shared Sub defaultKeyboard_onKeyDown(KeyName As String)
            If KeyName.ToLower().Contains("control") Then
                ctrl = True
            End If
            If Not ctrl Then
                If KeyName.ToLower().Contains("up") Then

                    up = True
                End If
                If KeyName.ToLower().Contains("down") Then
                    down = True
                End If
                If KeyName.ToLower().Contains("left") Then
                    left = True
                End If
                If KeyName.ToLower().Contains("right") Then
                    right = True
                End If
                If KeyName.ToLower().Contains("f10") Then
                    renderer.Dispose()
                    isrunning = False
                End If
            End If

        End Sub
        Shared mainmesh As Mesh
        Shared isrunning As Boolean = True
        Shared renderer As Renderer
        Shared left As Boolean = False
        Shared right As Boolean = False
        Shared up As Boolean = False
        Shared down As Boolean = False
        Private Shared Sub thetar()
            renderer.cameraPosition.Z -= 5.0F

            While isrunning
                'theobject.ownedVBO.rotation.X += .01f;
                Dim prevpos As Vector3D = renderer.cameraPosition
                'rotatingbuffer.rotation.Y = MathHelpers.DegreesToRadians(-90);
                'rotatingbuffer.rotation.Y += .01f;
                Dim cameraPosition As New Vector3D(0, 0, 0)

                Dim direction As Vector3D = Vector3D.ComputeRotation(New Vector3D(renderer.worldRotation.X, renderer.worldRotation.Y, renderer.worldRotation.Z))
                direction.Y *= -1
                Dim direction2 As Vector3D = Vector3D.ComputeRotation(New Vector3D(renderer.worldRotation.X, renderer.worldRotation.Y - 90, renderer.worldRotation.Z))

                If left Then

                    cameraPosition.X -= 0.1F * direction2.X
                    cameraPosition.Z -= 0.1F * direction2.Z
                End If
                If right Then
                    cameraPosition.X += 0.1F * direction2.X
                    cameraPosition.Z += 0.1F * direction2.Z
                End If
                If up Then
                    cameraPosition.Z += 0.1F * direction.Z
                    cameraPosition.X += 0.1F * direction.X
                End If
                If down Then
                    cameraPosition.Z -= 0.1F * direction.Z
                    cameraPosition.X -= 0.1F * direction.X
                End If

                ' collisiontester.Rotation = rotatingbuffer.rotation;
                renderer.cameraPosition += cameraPosition
                If collisiontester.Contains(renderer.cameraPosition) Then
                    renderer.cameraPosition = prevpos
                End If

                System.Threading.Thread.Sleep(10)
            End While
        End Sub
        Shared theobject As PhysicalObject
        Shared rotatingbuffer As VertexBuffer
    End Class
End Namespace
