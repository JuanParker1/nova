var base_url = $('body').data('baseurl')+"/";var sectionid=$('body').data('sectionid'); var appname='SOLINE';var app = angular.module('scmr.ctrl', ['ui.materialize','moment-picker','scmr.svc','ui-notification']);
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
.directive('ngConfirmClick', [
    function(){
        return {
            link: function ($scope, element, attr) {
                var msg = attr.ngConfirmClick || "Are you sure?";
                var clickAction = attr.confirmedClick;
                element.bind('click',function (event) {
                    if ( window.confirm(msg) ) {
                        $scope.$eval(clickAction);
                    }
                });
            }
        };
}])
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
 .config(function ($httpProvider) {
        var interceptor = function(userService, $q, $location)
        {
            return {
                request: function (config) 
                {
                	var currentUser = userService.GetCurrentUser(); 
                	if (currentUser != null) config.headers['Authorization'] = 'Bearer ' + currentUser.access_token; 
                	return config;
                },
                responseError : function(rejection)
                {
                    if (rejection.status === 401) {return $q.reject(rejection);}
                    if (rejection.status === 403) {return $q.reject(rejection);}
                    return $q.reject(rejection); 
                }
               };
         };      
        var params = ['userS', '$q', '$location'];
        interceptor.$inject = params;
        $httpProvider.interceptors.push(interceptor);
    })
  .filter('customCurrency', function(){
  return function(input, symbol, place){
    if(isNaN(input)){ return input;} else { var symbol = 'FCFA'; var place = place === undefined ? true : place;
      if(place === true){ return symbol + input;} else{ return input + symbol;} } }; 
})
.controller('CommonCtrl',function ($scope,$http,$timeout,Notification,vsom) {
	$scope.screendata={progress:false,bl:'',blconsigne:'',address:'',ref:'',ff:'', el:[],inv:[],pay:[],cn:[], mttc:'0',mht:'0',mtva:'0' , chassis:'', showvin : false , vinlib:'',actbtn:'indigo darken-3'};
	$scope.md={};
	$scope.blvin =[];
	$scope.lvinInit=function(lst,bl)
	{
		$scope.blvin=lst.Vehicules;$scope.screendata.bl=bl; $scope.screendata.ff=lst.FinFranchise; $scope.screendata.blconsigne=lst.Consignee;
	};
	$scope.bl=function(){
		if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
		$scope.screendata.bl=''; $scope.screendata.blconsigne='';  $scope.screendata.address=''; $scope.blvin=[];
		$scope.screendata.el=[]; $scope.screendata.mht=0; $scope.screendata.mttc=0; $scope.screendata.mtva=0;
		if($scope.md.numbl===undefined)
		{
			Notification.error({message: "Veuillez saisir un numéro de BL",  title: appname});
		}
		else
		{
			$scope.screendata.progress=true;
			var req=vsom.Bl($scope.md.numbl); 
			req.success(function(result) { $scope.screendata.progress=false; $scope.screendata.bl=result.data.NumBl; $scope.screendata.ff=result.data.FinFranchise;
				$scope.screendata.blconsigne=result.data.Consignee; $scope.screendata.address=result.data.Adresse; $scope.blvin=result.data.Vehicules;
				if(result.data.Vehicules.length==0)Notification.primary({message: "Il n'existe plus de chassis libre pour une quotation.",  title: appname});
				})
			.error(function(err){$scope.screendata.progress=false; Notification.error({message: 'Echec de la demande.',  title: appname});});
		}
		
	};
	$scope.fortesting=function(){
		if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
		if($scope.md.numbl===undefined || $scope.md.date===undefined){Notification.error({message: "Veuillez saisir un numéro de BL et indiquer la date de sortie",  title: appname}); return;}
		if( $scope.screendata.bl=="")
		{
			Notification.error({message: "Veuillez valider le bl",  title: appname}); 
		}
		else
		{ 
			 
			$scope.screendata.progress=true;
			var req=vsom.quote($scope.md); 
			req.success(function(result) {$scope.screendata.progress=false; $scope.screendata.el=result.data.Lignes; $scope.screendata.inv=result.data.OLignes; 
				$scope.screendata.mht=result.data.HT; $scope.screendata.mttc=result.data.MT; 
				$scope.screendata.ref=result.data.Ref; $scope.screendata.mtva=result.data.TVA;
				Notification.success({message:"Simulation effectuée. Veuillez consulter le détail",title:appname});})
			.error(function(err){$scope.screendata.progress=false;
				if(err.status==401){Notification.error({message: "Veuillez vous connecter de nouveau",  title: appname});}
				else
				Notification.error({message: 'Echec de la demande.',  title: appname});});
			
		}	
	};
	$scope.printing=function(){
		
	};
	$scope.gotesting=function(){
		if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
		if($scope.md.numbl===undefined || $scope.md.date===undefined )
		{
			Notification.error({message: "Veuillez saisir un numéro de BL et indiquer la date de sortie",  title: appname});
		}
		else
		{
			//if($scope.screendata.el.lenght==0) {Notification.error({message: "Veuillez effectuer la quotation avant de la validée",  title: appname}); return;}
			 
				var req=vsom.goquote($scope.md,$scope.screendata.ref,$scope.screendata.el,""); 
			req.success(function(result) {$scope.screendata.progress=false; 
				if(result.status===200){$scope.screendata.bl=''; $scope.screendata.blconsigne='';$scope.screendata.address=''; $scope.blvin=[]; 
				$scope.screendata.el=[]; $scope.screendata.mht=0; $scope.screendata.mttc=0; $scope.screendata.mtva=0;$scope.md={};
				Notification.success({message: "Votre demande de quotation est enregistrée sous la référence "+result.data+". Suivez son evolution dans la section Requetes.",  title: appname});}
				if(result.status===401){Notification.error({message: "Veuillez vous connecter de nouveau",  title: 'SOCOMAR APP'});} })
			.error(function(err){$scope.screendata.progress=false; Notification.error({message: 'Echec de la demande. Essayer de nouveau',  title: appname});});
		 
		}
	};
	
	$scope.click_step1=function(){
		if($scope.md.searchbl!= undefined && $scope.md.searchvin===undefined && $scope.screendata.showvin==false){
			 
			var reqSpec =$http({ url: base_url+"Search/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		 reqSpec.success(function(data){
		 if(data.res=='1') { $scope.screendata.showvin=true; $scope.screendata.actbtn='teal darken-2'; $scope.screendata.chassis=data.chassis; $scope.screendata.vinlib="Completez le chassis : "+data.chassis.replace(/.(?=.{6})/g, "*"); Notification.success({message: "Confirmer maintenant le chassis.",  title: appname}); } 
		 if(data.res=='-1') Notification.error({message: "Aucun connaissement trouvé.",  title: appname});
		 if(data.res==-2) Notification.error({message: "Données non consultables. Veuillez contacter nos bureaux",  title: appname}); });
		 reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec du chargement des données.',  title: appname});});
			
		}
		if($scope.md.searchvin!=undefined && $scope.screendata.showvin==true){
			if($scope.md.searchvin==$scope.screendata.chassis.substring(0,7)){
	       var reqSpec =$http({ url: base_url+"SearchElmt/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		   reqSpec.success(function(data){  if(data.res==1){ $scope.screendata=data; $scope.screendata.actbtn='indigo darken-3'; $scope.md={}; $scope.screendata.chassis=''; $scope.screendata.showvin=false;}
		 	if(data.res==-1) Notification.error({message: "Aucun connaissement trouvé ou vous ne disposez pas les accès.",  title: appname});  
		 	if(data.res==-2){ $scope.screendata.actbtn='indigo darken-3'; $scope.md={}; $scope.screendata.chassis=''; $scope.screendata.showvin=false; Notification.error({message: "les élements de ce connaissement ne sont pas encore disponible",  title: appname});} });
		 reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec chargement donnée.',  title: appname});});
		}
		else{
			Notification.error({message: "Chassis non confirmé.",  title: appname});
		}
			
	   }
	   if($scope.md.searchvin==undefined && $scope.screendata.showvin==true){
	   	Notification.error({message: "Chassis non valide.",  title: appname});
	   }
	};
	$scope.search=function(){
		if($scope.md.searchbl===undefined){Notification.error({message: 'Veuillez saisir un numero de connaissement.',  title: appname}); return;}
		if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
		$scope.screendata.progress=true;
		var req=vsom.finances($scope.md.searchbl); 
			req.success(function(result) {$scope.screendata.progress=false; $scope.screendata.bl=result.data.NumBl;
				$scope.screendata.blconsigne=result.data.Consignee; $scope.screendata.address=result.data.Adresse; $scope.screendata.inv=result.data.Invoices;
				$scope.screendata.pay=result.data.Payments; $scope.screendata.cn=result.data.CN;
			})
			.error(function(err){$scope.screendata.progress=false; Notification.error({message: 'Echec de la demande. Veuillez vérifier les données saisies',  title: appname});});
		 		
	};
	$scope.recap=function(){
		if($scope.md.start===undefined || $scope.md.end===undefined){Notification.error({message: 'Veuillez saisir une période valide',  title: appname}); return;}
		if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
		$scope.screendata.progress=true;
		var req=vsom.requests($scope.md); 
			req.success(function(result) { $scope.screendata.progress=false; 
				if(result.data.statut==0){Notification.error({message: "Veuillez vous connecter de nouveau",  title: appname});}
				else{$scope.screendata.el=result.data;}
			})
			.error(function(err){$scope.screendata.progress=false; Notification.error({message: 'Echec de la demande. Veuillez vérifier les données saisies',  title: appname});});
		 		
	};
});
