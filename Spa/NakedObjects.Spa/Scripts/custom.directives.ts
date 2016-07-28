﻿/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.services.viewmodelfactory.ts" />
/// <reference path="nakedobjects.viewmodels.ts" />
/// <reference path="typings/moment/moment.d.ts"/>

namespace NakedObjects {
    import Scope = angular.IScope;

    app.directive("customFullcalendar", (mask: IMask, $timeout: ng.ITimeoutService): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",

            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller

            link(scope: Scope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                const collection = (scope.$parent as INakedObjectsScope).collection;

                const content: any = element.append("<div class='content'></div>");

                let id: number = null;

                function setup() {

                    if (_.every(collection.items, i => i.tableRowViewModel)) {
                        clearInterval(id);

                        let lastDate: Date;

                        const events = _.map(collection.items, i => {
                                const date = i.tableRowViewModel.properties[2].value as Date;

                                if (!lastDate || date > lastDate) {
                                    lastDate = date;
                                }

                                return {
                                    title: i.title,
                                    start: date.toISOString(),
                                    allDay: true,
                                    color: (<any>i).color,
                                    vm: i,
                                    url: "empty"
                                };
                            });

                        content.fullCalendar({
                            events: events,
                            eventClick: (evt: any) => {
                                $timeout(() => evt.vm.doClick(false));
                                return false;
                            }
                        });

                        content.fullCalendar("gotoDate", lastDate.toISOString());                       
                    }
                }

                id = setInterval(setup, 100);
            }
        };
    });

    app.directive("customWorkordercalendar", (mask: IMask, $timeout: ng.ITimeoutService, urlManager : IUrlManager): ng.IDirective => {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: "A",

            // to make sure dynamic ids on element get picked up
            transclude: true,
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller

            link(scope: Scope, element: ng.IAugmentedJQuery, attrs: ng.IAttributes, ngModel: ng.INgModelController) {

                const object = (scope.$parent as INakedObjectsScope).object;
                const collection = _.find(object.collections, c => c.title == 'Work Order Routings');
                collection.doTable();
                collection.refresh(urlManager.getRouteData().pane()[object.onPaneId], true);


                const content: any = element.append("<div class='content'></div>");

                let id: number = null;

                function setup() {

                    if (collection.items && _.every(collection.items, i => i.tableRowViewModel)) {
                        clearInterval(id);

                        let lastDate: Date;

                        if (collection.items.length === 0) {
                            lastDate = new Date(Date.now());
                        }

                        const events = _.map(collection.items, i => {
                            const props = i.tableRowViewModel.properties;
                            const start = _.find(props, p => p.title == 'Scheduled Start Date').value as Date;
                            const end = _.find(props, p => p.title == 'Scheduled End Date').value as Date;
                            if (!lastDate || start > lastDate) {
                                lastDate = start;
                            }

                            return {
                                title: i.title,
                                start: start.toISOString(),
                                end: end.toISOString(),
                                color: (<any>i).color,
                                vm: i,
                                url: "empty"
                            };
                        });

                        content.fullCalendar({
                            header: { center: 'month agendaWeek agendaDay' },
                            defaultView: 'agendaDay',
                            events: events,
                            eventClick: (evt: any) => {
                                $timeout(() => evt.vm.doClick(false));
                                return false;
                            }
                        });
                        content.fullCalendar("gotoDate", lastDate.toISOString());
                    }
                }

                id = setInterval(setup, 100);
            }
        };
    });


}