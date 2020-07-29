Imports System
Imports System.IO
Imports System.Numerics
Imports Keeper.Manage

Namespace Keeper
    Class CryptedData
        Public DataToModify As String
        Public Sub New(ByVal Data As String)
            DataToModify = Data
        End Sub
        Public Function Encrypt() As String
            Dim Result As Char() = Me.DataToModify.ToArray()
            Dim NewValue As Integer = 0

            Console.WriteLine(Me.DataToModify)
            For i As Integer = 0 To Result.Length - 1 Step 1
                NewValue = Asc(Result(i)) - 9
                Result(i) = Chr(NewValue)
            Next
            Result.ToString()
            Return Result
        End Function
        Public Function Decrypt() As String
            Dim Result As Char() = Me.DataToModify.ToArray()
            Dim NewValue As Integer = 0

            For i As Integer = 0 To Result.Length - 1 Step 1
                NewValue = Asc(Result(i)) + 9
                Result(i) = Chr(NewValue)
            Next
            Result.ToString()
            Return Result
        End Function
    End Class
    Module Program
        Function FirstTime() As Boolean
            Dim path As String = Environment.GetFolderPath(5) + "\Keeper"
            Dim files() As String = New String() {}

            If Directory.Exists(path) = False Then
                Directory.CreateDirectory(path)
            End If
            files = Directory.GetFiles(path)
            For Each file As String In files
                If path + "\access.txt" = file Then
                    Return False
                End If
            Next
            Return True
        End Function
        Function CreateMaster_Password() As String
            Dim Answer As String = Console.ReadLine()
            Dim Confirm As String = String.Empty
            Dim Match As Boolean = False

            While Match = False
                For i As Integer = 1 To 3 Step 0
                    Console.WriteLine("Please Confirm Password:")
                    Confirm = Console.ReadLine()
                    If Confirm = Answer Then
                        Match = True
                        GoTo Final
                    End If
                    Console.WriteLine("Password does not match: " + (3 - i).ToString + " tries left")
                    i += 1
                Next
                Console.WriteLine("Starting over with a new password")
                Answer = Console.ReadLine()
Final:
            End While
            Return Answer
        End Function
        Sub SaveMaster_Password(Master As String)
            Dim path As String = Environment.GetFolderPath(5) + "\Keeper\access.txt"
            Dim CryptedMaster As CryptedData = New CryptedData(Master)

            Using fd As New StreamWriter(File.Open(path, FileMode.OpenOrCreate))
                fd.WriteLine(CryptedMaster.Encrypt())
            End Using
            Console.WriteLine("Password Saved !")
        End Sub
        Function GetPassword(Filename As String) As String
            Dim Result As String = String.Empty
            Dim Path As String = Environment.GetFolderPath(5) + "\Keeper\" + Filename + ".txt"

            Using Fd As New StreamReader(File.Open(Path, FileMode.Open))
                Result = Fd.ReadLine()
            End Using
            Return Result
        End Function
        Sub GetAccess()
            Dim Input As String = String.Empty
            Dim Crypted As String = GetPassword("access")
            Dim DecryptedMaster As CryptedData = New CryptedData(Crypted)
            Dim Password As String = DecryptedMaster.Decrypt()

            For tries As Integer = 1 To 3 Step 1
                Input = Console.ReadLine()
                If Input = Password Then
                    Console.WriteLine("Access granted !")
                    Return
                End If
                Console.WriteLine("Password does not match: " + (3 - tries).ToString + " Remaining.")
            Next
            Console.WriteLine("All tries have been used exiting application...")
            Environment.Exit(-1)
        End Sub
        Sub Main(args As String())
            Dim Master As String = String.Empty
            Dim CorePart As Managing = New Managing()

            If FirstTime() = True Then
                Console.WriteLine("Welcome to Keeper a tools to keep your password safe." + Environment.NewLine + "Please create your master password:")
                Master = CreateMaster_Password()
                SaveMaster_Password(Master)
            Else
                Console.WriteLine("Welcome back to Keeper !" + Environment.NewLine + "Enter your master password:")
                GetAccess()
            End If
            CorePart.Launch()
        End Sub
    End Module
End Namespace
