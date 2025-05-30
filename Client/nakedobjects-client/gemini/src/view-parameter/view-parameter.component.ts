import { Component, Input } from '@angular/core';
import { DialogViewModel, ParameterViewModel } from '@nakedobjects/view-models';

@Component({
    selector: 'nof-view-parameter',
    templateUrl: 'view-parameter.component.html',
    styleUrls: ['view-parameter.component.css'],
    standalone: false
})
export class ViewParameterComponent {

    @Input({required : true})
    parent!: DialogViewModel;

    @Input({required : true})
    parameter!: ParameterViewModel;

    get title() {
        return this.parameter.title;
    }

    get parameterPaneId() {
        return this.parameter.paneArgId;
    }

    get parameterType() {
        return this.parameter.type;
    }

    get parameterReturnType() {
        return this.parameter.returnType;
    }

    get formattedValue() {
        return this.parameter.formattedValue;
    }

    get value() {
        return this.parameter.value;
    }

    get format() {
        return this.parameter.format;
    }

    get isMultiline() {
        return !(this.parameter.multipleLines === 1);
    }

    get multilineHeight() {
        return `${this.parameter.multipleLines * 20}px`;
    }

    get color() {
        return this.parameter.color;
    }
}
