Imports System
Imports System.IO
Imports System.Numerics
Imports Keeper.Keeper

Namespace Manage
    Class Managing
        Public Delegate Function RunIt() As Integer
        Public Structure Instruction
            Public Instr As String
            Public Fct As RunIt
        End Structure
        Public Sub WritePassword(Title As String, Password As String)
            Dim Path As String = $"{Environment.GetFolderPath(5)}\Keeper\{Title}.txt"
            Dim CrypedPasswd As CryptedData = New CryptedData(Password)

            Using Fd As New StreamWriter(File.Open(Path, FileMode.OpenOrCreate))
                Fd.WriteLine(CrypedPasswd.Encrypt())
            End Using
            Console.WriteLine("Password saved !")
        End Sub
        Public Sub GeneratePassword()
            Dim Password(11) As Char
            Dim Title As String = String.Empty
            Dim Rand As Random = New Random()
            Dim Val As Integer = 0
            Dim Result As String = String.Empty

            Console.WriteLine("Please enter the title of the application that you want to save your password from:")
            Title = Console.ReadLine()
            For i As Integer = 0 To 11 Step 1
                Val = Rand.Next(1, 4)
                If Val = 1 Then
                    Val = Rand.Next(97, 123)
                ElseIf Val = 2 Then
                    Val = Rand.Next(65, 91)
                Else
                    Val = Rand.Next(48, 58)
                End If
                Password(i) = Chr(Val)
            Next
            Result = New String(Password)
            WritePassword(Title, Result)
        End Sub
        Public Sub CreatePassword()
            Dim Confirm As String = String.Empty
            Dim Password As String = String.Empty
            Dim Title As String = String.Empty

            Console.WriteLine("Please enter the title of the application that you want to save your password from:")
            Title = Console.ReadLine()
            While True
                Console.WriteLine("Type the password please:")
                Password = Console.ReadLine()
                Console.WriteLine("Confirm Password:")
                For i As Integer = 1 To 3 Step 1
                    Confirm = Console.ReadLine()
                    If Confirm = Password Then
                        GoTo Pass
                    End If
                    Console.WriteLine("Password does not match -> " + (3 - i).ToString + " Remaining")
                Next
                Console.WriteLine("No tries remaining reseting password...")
            End While
Pass:
            WritePassword(Title, Password)
        End Sub
        Public Sub AddPassword()
            Dim Ans As String = String.Empty

            Console.WriteLine($"Would you like to generate your password or create it ?{Environment.NewLine}Type ""create"" or ""generate""")
            Ans = Console.ReadLine()
            If Ans.ToLower = "generate" Then
                GeneratePassword()
            ElseIf Ans.ToLower = "create" Then
                CreatePassword()
            Else
                Console.Error.WriteLine("Error: " + Ans + " is not allowed")
            End If
        End Sub
        Public Function GetName(Elem As String) As String
            Dim Array As Char() = Elem.ToArray()
            Dim Result As String = String.Empty
            Dim i As Integer = Array.Length - 1

            While Array(i) <> "."
                i -= 1
            End While
            i -= 1
            While Array(i) <> "\"
                Result = Result.Insert(0, Array(i).ToString)
                i -= 1
            End While
            Return Result
        End Function
        Public Function RecoverPassword(Path As String) As String
            Dim Tools As CryptedData = New CryptedData(String.Empty)

            Using Fd As New StreamReader(File.Open(Path, FileMode.Open))
                Tools.DataToModify = Fd.ReadLine()
            End Using
            Return Tools.Decrypt()
        End Function
        Public Sub FetchPassword()
            Dim Files As String() = New String() {}
            Dim Name As String = String.Empty
            Dim Input As String = String.Empty
            Dim Password As String = String.Empty

            Try
                Files = Directory.GetFiles($"{Environment.GetFolderPath(5)}\Keeper")
            Catch ex As Exception
                Console.Error.WriteLine($"Fatal Error: {ex.Message}")
                Environment.Exit(-1)
            End Try
            If Files.Length = 0 Then
                Console.Error.WriteLine("Error you need to have saved at least one password before")
                Return
            End If
            Console.WriteLine("Please Type the exact name of the password you want to retrieve:")
            For Each File In Files
                If GetName(File) <> "access" Then
                    Console.WriteLine(GetName(File))
                End If
            Next
            While True
                Input = Console.ReadLine()
                For Each Elem In Files
                    Name = GetName(Elem)
                    If Name = Input And Name <> "access" Then
                        Password = RecoverPassword(Elem)
                        Console.WriteLine(Password)
                        Return
                    End If
                Next
                Console.WriteLine("Error: " + Input + " is not valid")
            End While
        End Sub
        Public Sub RemovePasswd()
            Dim Files() As String = New String() {}
            Dim Input As String = String.Empty
            Dim Name As String = String.Empty

            Try
                Files = Directory.GetFiles($"{Environment.GetFolderPath(5)}\Keeper")
            Catch ex As Exception
                Console.Error.WriteLine($"Fatal Error: {ex.Message}")
                Environment.Exit(-1)
            End Try
            If Files.Length = 1 Then
                Console.WriteLine("You do not have any password to remove")
                Return
            End If
            Console.WriteLine("Enter the name of the element that you want to remove")
            For Each Current In Files
                If Me.GetName(Current) <> "access" Then
                    Console.WriteLine(Me.GetName(Current))
                End If
            Next
            While True
                Input = Console.ReadLine()
                For Each Current In Files
                    Name = GetName(Current)
                    If Input = Name And Input <> "access" Then
                        File.Delete(Current)
                        Console.WriteLine($"{Name} Sucessfully deleted")
                        Return
                    End If
                Next
                Console.Error.WriteLine($"Error: {Input} file does not exist")
            End While
        End Sub
        Public Function Quit() As Integer
            Return 1
        End Function
        Public Function Add() As Integer
            Me.AddPassword()
            Return 0
        End Function
        Public Function Fetch() As Integer
            Me.FetchPassword()
            Return 0
        End Function
        Public Function Remove() As Integer
            Me.RemovePasswd()
            Return 0
        End Function

        Public Function CreateTab() As Instruction()
            Dim Tab(3) As Instruction

            Tab(0).Instr = "quit"
            Tab(0).Fct = AddressOf Quit
            Tab(1).Instr = "add"
            Tab(1).Fct = AddressOf Add
            Tab(2).Instr = "fetch"
            Tab(2).Fct = AddressOf Fetch
            Tab(3).Instr = "remove"
            Tab(3).Fct = AddressOf Remove
            Return Tab
        End Function
        Public Sub Launch()
            Dim Choice As String = String.Empty
            Dim Tab As Instruction() = CreateTab()
            Dim Find As Boolean = False

            Console.WriteLine($"You're in !{Environment.NewLine}Would you like to add a password or gather one, you can even remove one ?")
            While Choice.ToLower <> "quit"
                Console.WriteLine("Type ""add"", ""fetch"" or ""remove"", you can type ""quit"" to close this application")
                Choice = Console.ReadLine()
                For Each Instruct In Tab
                    If Choice.ToLower = Instruct.Instr Then
                        Find = True
                        If Instruct.Fct.Invoke() = 1 Then
                            Environment.Exit(0)
                        End If
                    End If
                Next
                If Find = False Then
                    Console.Error.WriteLine($"Error command unknown: {Choice}")
                End If
            End While
        End Sub
    End Class
End Namespace