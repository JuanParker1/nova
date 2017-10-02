var base_url = $('body').data('baseurl')+"/";
var sectionid=$('body').data('sectionid');
var app = angular.module('socomar', ['ui.materialize','ui-notification']);
app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if(event.which === 13) {
                scope.$apply(function (){
                    scope.$eval(attrs.ngEnter);
                });
 
                event.preventDefault();
            }
        });
    };
})
.config(function(NotificationProvider) {
        NotificationProvider.setOptions({
            delay: 10000,
            startTop: 40,
            startRight: 10,
            verticalSpacing: 20,
            horizontalSpacing: 20,
            positionX: 'right',
            positionY: 'top'
        });
    })
.controller('WelcomeCtrl',function ($scope,$http,$timeout,Notification) {
	
	$scope.save=function(){};
})
.controller('EtsCtrl',function ($scope,$http,$timeout,Notification) {
	
	$scope.screendata={bl:'S33333',blconsigne:'',address:'',phone:''};
	$scope.md={};
	$scope.search=function(){
		
	};
})
.controller('PartCtrl',function ($scope,$http,$timeout,Notification) {
	$scope.screendata={bl:'',blconsigne:'',address:'',phone:'', el:[], mttc:'0',mht:'0',mtva:'0' , chassis:'', showvin : false , vinlib:'Complete missed  VIN Number value : '};
	$scope.md={};
	$scope.click_step1=function(){
		if($scope.md.searchbl!= undefined && $scope.md.searchvin===undefined && $scope.screendata.showvin==false){
			 
			var reqSpec =$http({ url: base_url+"Search/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		 reqSpec.success(function(data){
		 if(data.res=='1') { $scope.screendata.showvin=true; $scope.screendata.chassis=data.chassis; $scope.screendata.vinlib="Complete VIN Number : "+data.chassis.replace(/.(?=.{6})/g, "*"); Notification.success({message: "Confirmer maintenant le chassis.",  title: 'SOCOMAR APP'}); } 
		 if(data.res=='-1') Notification.error({message: "Aucun connaissement trouvé.",  title: 'SOCOMAR APP'});
		 if(data.res==-2) Notification.error({message: "Données non consultables. Veuillez contacter nos bureaux",  title: 'SOCOMAR APP'}); });
		 reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec chargement donnée.',  title: 'SOCOMAR APP'});});
			
		}
		if($scope.md.searchvin!=undefined && $scope.screendata.showvin==true){
			if($scope.md.searchvin==$scope.screendata.chassis.substring(0,7)){
	       var reqSpec =$http({ url: base_url+"SearchElmt/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		   reqSpec.success(function(data){  if(data.res==1){ $scope.screendata=data; $scope.md={}; $scope.screendata.chassis=''; $scope.screendata.showvin=false;}
		 	if(data.res==-1) Notification.error({message: "Aucun connaissement trouvé ou vous ne disposez pas les accès.",  title: 'SOCOMAR APP'});  
		 	if(data.res==-2) Notification.error({message: "les élements de ce connaissement ne sont pas encore disponible",  title: 'SOCOMAR APP'}); });
		 reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec chargement donnée.',  title: 'SOCOMAR APP'});});
		}
		else{
			Notification.error({message: "Chassis non confirmé.",  title: 'SOCOMAR APP'});
		}
			
	   }
	   if($scope.md.searchvin==undefined && $scope.screendata.showvin==true){
	   	Notification.error({message: "Chassis non valide.",  title: 'SOCOMAR APP'});
	   }
	};
	$scope.search=function(){
		if($scope.md.searchbl===undefined){Notification.error({message: 'Veuillez saisir un numero de connaissement.',  title: 'SOCOMAR APP'});}
		var reqSpec =$http({ url: base_url+"Search/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		 reqSpec.success(function(data){ if(data.res==1) $scope.screendata=data; 
		 	if(data.res==-1) Notification.error({message: "Aucun connaissement trouvé ou vous ne disposez pas les accès.",  title: 'SOCOMAR APP'});  
		 	if(data.res==-2) Notification.error({message: "les élements de ce connaissement ne sont pas encore disponible",  title: 'SOCOMAR APP'}); });
		 reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec chargement donnée.',  title: 'SOCOMAR APP'});});
				
	};
});