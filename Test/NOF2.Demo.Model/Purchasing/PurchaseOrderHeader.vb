﻿Namespace AW.Types

	Partial Public Class PurchaseOrderHeader

		Implements ITitledObject, INotEditableOncePersistent

		''<Hidden>
		Public Property PurchaseOrderID() As Integer

#Region "RevisionNumber"
		Public Property mappedRevisionNumber As Byte
		Friend myRevisionNumber As WholeNumber

		<DemoProperty(Order:=90)>
		Public ReadOnly Property RevisionNumber As WholeNumber
			Get
				myRevisionNumber = If(myRevisionNumber, New WholeNumber(mappedRevisionNumber, Sub(v) mappedRevisionNumber = CType(v, Byte)))
				Return myRevisionNumber
			End Get
		End Property

		Public Sub AboutRevisionNumber(a As FieldAbout, RevisionNumber As WholeNumber)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property ShipMethodID() As Integer

		<DemoProperty(Order:=22)>
		Public Overridable Property ShipMethod() As ShipMethod

#Region "Details (Collection)"
		Public Overridable Property mappedDetails As ICollection(Of PurchaseOrderDetail) = New List(Of PurchaseOrderDetail)()

		Private myDetails As InternalCollection

		'''<TableView(True, "OrderQty", "Product", "UnitPrice", "LineTotal")>
		<DemoProperty(Order:=1)>
		Public ReadOnly Property Details As InternalCollection
			Get
				myDetails = If(myDetails, New InternalCollection(Of PurchaseOrderDetail)(mappedDetails))
				Return myDetails
			End Get
		End Property

		Public Sub AboutDetails(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case Else
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property VendorID() As Integer

		<DemoProperty(Order:=1)>
		Public Overridable Property Vendor() As Vendor

		''<Hidden>
		Public Property Status() As Byte

		Public ReadOnly Property StatusAsEnum As TextString
			Get
				Return New TextString([Enum].GetName(GetType(POStatus), Status))
			End Get
		End Property

		Public Sub AboutStatusAsEnum(a As FieldAbout)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
					a.Name = "Status"
			End Select
		End Sub

#Region "OrderDate"
		Public Property mappedOrderDate As Date
		Friend myOrderDate As NODate

		<DemoProperty(Order:=11)>
		Public ReadOnly Property OrderDate As NODate
			Get
				myOrderDate = If(myOrderDate, New NODate(mappedOrderDate, Sub(v) mappedOrderDate = v))
				Return myOrderDate
			End Get
		End Property

		Public Sub AboutOrderDate(a As FieldAbout, OrderDate As NODate)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "ShipDate"
		Public Property mappedShipDate As DateTime?
		Friend myShipDate As NODateNullable

		<DemoProperty(Order:=20)>
		Public ReadOnly Property ShipDate As NODateNullable
			Get
				myShipDate = If(myShipDate, New NODateNullable(mappedShipDate, Sub(v) mappedShipDate = v))
				Return myShipDate
			End Get
		End Property

		Public Sub AboutShipDate(a As FieldAbout, ShipDate As NODateNullable)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "SubTotal"
		Public Property mappedSubTotal As Decimal
		Friend mySubTotal As Money

		<DemoProperty(Order:=31)>
		Public ReadOnly Property SubTotal As Money
			Get
				mySubTotal = If(mySubTotal, New Money(mappedSubTotal, Sub(v) mappedSubTotal = v))
				Return mySubTotal
			End Get
		End Property

		Public Sub AboutSubTotal(a As FieldAbout, SubTotal As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "TaxAmt"
		Public Property mappedTaxAmt As Decimal
		Friend myTaxAmt As Money

		<DemoProperty(Order:=32)>
		Public ReadOnly Property TaxAmt As Money
			Get
				myTaxAmt = If(myTaxAmt, New Money(mappedTaxAmt, Sub(v) mappedTaxAmt = v))
				Return myTaxAmt
			End Get
		End Property

		Public Sub AboutTaxAmt(a As FieldAbout, TaxAmt As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "Freight"
		Public Property mappedFreight As Decimal
		Friend myFreight As Money

		<DemoProperty(Order:=33)>
		Public ReadOnly Property Freight As Money
			Get
				myFreight = If(myFreight, New Money(mappedFreight, Sub(v) mappedFreight = v))
				Return myFreight
			End Get
		End Property

		Public Sub AboutFreight(a As FieldAbout, Freight As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

#Region "TotalDue"
		Public Property mappedTotalDue As Decimal
		Friend myTotalDue As Money

		<DemoProperty(Order:=34)>
		Public ReadOnly Property TotalDue As Money
			Get
				myTotalDue = If(myTotalDue, New Money(mappedTotalDue, Sub(v) mappedTotalDue = v))
				Return myTotalDue
			End Get
		End Property

		Public Sub AboutTotalDue(a As FieldAbout, TotalDue As Money)
			Select Case a.TypeCode
				Case AboutTypeCodes.Name
				Case AboutTypeCodes.Usable
				Case AboutTypeCodes.Valid
				Case Else
			End Select
		End Sub
#End Region

		''<Hidden>
		Public Property OrderPlacedByID() As Integer

		<DemoProperty(Order:=12)>
		Public Overridable Property OrderPlacedBy() As Employee

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

		Public Function Title() As Title Implements ITitledObject.Title
			Return New Title(ToString())
		End Function

		Public Overrides Function ToString() As String
			Return $"PO from {Vendor}, {OrderDate}"
		End Function

	End Class

	Public Enum POStatus
		Pending = 1
		Approved = 2
		Rejected = 3
		Complete = 4
	End Enum
End Namespace