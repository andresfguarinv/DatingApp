import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';

@Injectable()

export class PreventUnSavedChanges implements CanDeactivate<any> {
    canDeactivate(component: any) {
        if (component.editForm.dirty) {
            return confirm('Are you sure you want to continue ? Any unsaved changes will be lost!');
        }
        return true;
    }
}