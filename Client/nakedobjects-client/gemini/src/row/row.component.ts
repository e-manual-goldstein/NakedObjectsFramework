﻿import { Component, ElementRef, Input, QueryList, ViewChildren } from '@angular/core';
import { copy, DragAndDropService, IDraggableViewModel, ItemViewModel, RecentItemViewModel, TableRowColumnViewModel } from '@nakedobjects/view-models';
import { focus } from '../helpers-components';

@Component({
    // eslint-disable-next-line @angular-eslint/component-selector
    selector: '[nof-row]',
    templateUrl: 'row.component.html',
    styleUrls: ['row.component.css'],
    standalone: false
})
export class RowComponent {

    constructor(
        private readonly dragAndDrop: DragAndDropService,
    ) { }

    @Input({required : true})
    item!: ItemViewModel;

    @Input({required : true})
    row!: number;

    @Input({required : true})
    withCheckbox!: boolean;

    @Input({required : true})
    isTable!: boolean;

    @ViewChildren('focus')
    rowChildren?: QueryList<ElementRef>;

    get id() {
        return `${this.item.id || 'item'}${this.item.paneId}-${this.row}`;
    }

    get color() {
        return this.item.color;
    }

    get selected() {
        return this.item.selected;
    }

    get title() {
        return this.item.title;
    }

    get friendlyName() {
        return this.item instanceof RecentItemViewModel ? this.item.friendlyName : '';
    }

    tabIndexFirstColumn(i: number | string) {
        if (this.isTable) {
            if (this.hasTableTitle()) {
                return i === 'title' ? 0 : -1;
            } else if (this.friendlyName) {
                return i === 'fname' ? 0 : -1;
            } else if (i === 0) {
                return 0;
            }
        }
        return -1;
    }

    tableTitle = () => this.item.tableRowViewModel ? this.item.tableRowViewModel.title : this.title;
    hasTableTitle = () => this.item.tableRowViewModel?.showTitle || !!(this.item instanceof RecentItemViewModel && this.item.title);
    tableProperties = (): TableRowColumnViewModel[] => this.item.tableRowViewModel?.properties ?? [];

    propertyType = (property: TableRowColumnViewModel) => property.type;
    propertyValue = (property: TableRowColumnViewModel) => property.value;
    propertyFormattedValue = (property: TableRowColumnViewModel) => property.formattedValue;
    propertyReturnType = (property: TableRowColumnViewModel) => property.returnType;

    doClick = (right?: boolean) => this.item.doClick(right);

    copy(event: KeyboardEvent, item: IDraggableViewModel) {
        copy(event, item, this.dragAndDrop);
    }

    focus() {
        return !!this.rowChildren && this.rowChildren.length > 0 && focus(this.rowChildren.first);
    }
}
