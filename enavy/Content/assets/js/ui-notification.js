angular.module("ui-notification",[]),angular.module("ui-notification").provider("Notification",function(){this.options={delay:5e3,startTop:10,startRight:10,verticalSpacing:10,horizontalSpacing:10,positionX:"right",positionY:"top",replaceMessage:!1,templateUrl:"angular-ui-notification.html",onClose:void 0,closeOnClick:!0,maxCount:0,container:"body",priority:10},this.setOptions=function(t){if(!angular.isObject(t))throw new Error("Options should be an object!");this.options=angular.extend({},this.options,t)},this.$get=["$timeout","$http","$compile","$templateCache","$rootScope","$injector","$sce","$q","$window",function(t,i,e,n,o,s,r,a,l){var p=this.options,c=p.startTop,u=p.startRight,d=p.verticalSpacing,f=p.horizontalSpacing,m=p.delay,g=[],h=!1,y=function(s,y){function C(i){function n(t){["-webkit-transition","-o-transition","transition"].forEach(function(i){C.css(i,t)})}var o=s.scope.$new();o.message=r.trustAsHtml(s.message),o.title=r.trustAsHtml(s.title),o.t=s.type.substr(0,1),o.delay=s.delay,o.onClose=s.onClose;var a=function(t,i){return t._priority-i._priority},m=function(t,i){return i._priority-t._priority},y=function(){var t=0,i=0,e=c,n=u,o=[];"top"===s.positionY?g.sort(a):"bottom"===s.positionY&&g.sort(m);for(var r=g.length-1;r>=0;r--){var l=g[r];if(s.replaceMessage&&r<g.length-1)l.addClass("killed");else{var h=parseInt(l[0].offsetHeight),y=parseInt(l[0].offsetWidth),C=o[l._positionY+l._positionX];v+h>window.innerHeight&&(C=c,i++,t=0);var v=e=C?0===t?C:C+d:c,_=n+i*(f+y);l.css(l._positionY,v+"px"),"center"==l._positionX?l.css("left",parseInt(window.innerWidth/2-y/2)+"px"):l.css(l._positionX,_+"px"),o[l._positionY+l._positionX]=v+h,p.maxCount>0&&g.length>p.maxCount&&0===r&&l.scope().kill(!0),t++}}},C=e(i)(o);C._positionY=s.positionY,C._positionX=s.positionX,C._priority=s.priority,C.addClass(s.type);var _=function(t){t=t.originalEvent||t,("click"===t.type||"opacity"===t.propertyName&&t.elapsedTime>=1)&&(o.onClose&&o.$apply(o.onClose(C)),C.remove(),g.splice(g.indexOf(C),1),o.$destroy(),y())};s.closeOnClick&&(C.addClass("clickable"),C.bind("click",_)),C.bind("webkitTransitionEnd oTransitionEnd otransitionend transitionend msTransitionEnd",_),angular.isNumber(s.delay)&&t(function(){C.addClass("killed")},s.delay),n("none"),angular.element(document.querySelector(s.container)).append(C);var k=-(parseInt(C[0].offsetHeight)+50);if(C.css(C._positionY,k+"px"),g.push(C),"center"==s.positionX){var w=parseInt(C[0].offsetWidth);C.css("left",parseInt(window.innerWidth/2-w/2)+"px")}t(function(){n("")}),o._templateElement=C,o.kill=function(i){i?(o.onClose&&o.$apply(o.onClose(o._templateElement)),g.splice(g.indexOf(o._templateElement),1),o._templateElement.remove(),o.$destroy(),t(y)):o._templateElement.addClass("killed")},t(y),h||(angular.element(l).bind("resize",function(i){t(y)}),h=!0),v.resolve(o)}var v=a.defer();"object"==typeof s&&null!==s||(s={message:s}),s.scope=s.scope?s.scope:o,s.template=s.templateUrl?s.templateUrl:p.templateUrl,s.delay=angular.isUndefined(s.delay)?m:s.delay,s.type=y||s.type||p.type||"",s.positionY=s.positionY?s.positionY:p.positionY,s.positionX=s.positionX?s.positionX:p.positionX,s.replaceMessage=s.replaceMessage?s.replaceMessage:p.replaceMessage,s.onClose=s.onClose?s.onClose:p.onClose,s.closeOnClick=null!==s.closeOnClick&&void 0!==s.closeOnClick?s.closeOnClick:p.closeOnClick,s.container=s.container?s.container:p.container,s.priority=s.priority?s.priority:p.priority;var _=n.get(s.template);return _?C(_):i.get(s.template,{cache:!0}).then(C)["catch"](function(t){throw new Error("Template ("+s.template+") could not be loaded. "+t)}),v.promise};return y.primary=function(t){return this(t,"primary")},y.error=function(t){return this(t,"error")},y.success=function(t){return this(t,"success")},y.info=function(t){return this(t,"info")},y.warning=function(t){return this(t,"warning")},y.clearAll=function(){angular.forEach(g,function(t){t.addClass("killed")})},y}]}),angular.module("ui-notification").run(["$templateCache",function(t){t.put("angular-ui-notification.html",'<div class="ui-notification"><h3 ng-show="title" ng-bind-html="title"></h3><div class="message" ng-bind-html="message"></div></div>')}]);