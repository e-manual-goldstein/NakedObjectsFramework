﻿Namespace AW.Types
	'<Named("Contact")>
	Partial Public Class BusinessEntityContact

		Implements ITitledObject, INotEditableOncePersistent

		Public Property BusinessEntityID() As Integer

		Public Overridable Property BusinessEntity() As BusinessEntity

		Public Sub AboutBusinessEntity(a As FieldAbout)
			Select Case a.TypeCode
				Case Else
					a.Visible = False
			End Select
		End Sub

		Public Property PersonID() As Integer

		<DemoProperty(Order:=1)>
		Public Overridable Property Person() As Person

		Public Property ContactTypeID() As Integer

		<DemoProperty(Order:=2)>
		Public Overridable Property ContactType() As ContactType

#Region "ModifiedDate"
		Public Property mappedModifiedDate As Date
		Friend myModifiedDate As TimeStamp

		<DemoProperty(Order:=99)>
		Public ReadOnly Property ModifiedDate As TimeStamp
			Get
				myModifiedDate = If(myModifiedDate, New TimeStamp(mappedModifiedDate, Sub(v) mappedModifiedDate = v))
				Return myModifiedDate
			End Get
		End Property

		Public Sub AboutModifiedDate(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Usable
					a.Usable = False
			End Select
		End Sub
#End Region

		Public Property RowGuid() As Guid

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return Person.ToString()
		End Function
	End Class
End Namespace