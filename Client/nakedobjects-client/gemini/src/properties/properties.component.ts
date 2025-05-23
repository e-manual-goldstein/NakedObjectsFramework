﻿import { Component, Input, QueryList, ViewChildren } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DomainObjectViewModel, PropertyViewModel } from '@nakedobjects/view-models';
import some from 'lodash-es/some';
import { EditPropertyComponent } from '../edit-property/edit-property.component';

@Component({
    selector: 'nof-properties',
    templateUrl: 'properties.component.html',
    styleUrls: ['properties.component.css'],
    standalone: false
})
export class PropertiesComponent {

    @Input()
    parent?: DomainObjectViewModel;

    @Input()
    form?: FormGroup;

    @Input({required : true})
    properties!: PropertyViewModel[];

    @ViewChildren(EditPropertyComponent)
    propComponents?: QueryList<EditPropertyComponent>;

    classes(prop: PropertyViewModel) {
        const hint = prop.presentationHint ?? '';
        return `property ${hint}`.trim();
    }

    focus() {
        const prop = this.propComponents;
        if (prop && prop.length > 0) {
            // until first element returns true
            return some(prop.toArray(), i => i.focus());
        }
        return false;
    }
}
