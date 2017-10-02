var base_url = $('body').data('baseurl')+"/";var sectionid=$('body').data('sectionid'); 
var appname='SOLINE';var app = angular.module('scmr.ctrl', ['ui.materialize', 'scmr.svc','ui-notification']);
app.config(function(NotificationProvider) {
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
   });
app.controller('CommonCtrl',function ($scope,$http,$timeout,Notification,vsom) {
	$scope.screendata={progress:false,bl:'',blconsigne:'',address:'',ref:'',ff:'', el:[],inv:[],pay:[],cn:[], mttc:'0',mht:'0',mtva:'0' , chassis:'', showvin : false , vinlib:'',actbtn:'indigo darken-3'};
	$scope.md={};
	$scope.blvin =[];
	$scope.lvinInit=function(lst)
	{
		$scope.blvin=lst;
	};
	$scope.click_step1=function(){
		if($scope.md.searchbl!= undefined && $scope.md.searchvin===undefined && $scope.screendata.showvin==false){
			 if($scope.screendata.progress==true){Notification.primary({message: "Une demande est déja en cours.",  title: appname}); return;}
			 
			$scope.screendata.progress=true; var req=vsom.Bl($scope.md.searchbl); 
			req.success(function(result) 
			{ $scope.screendata.progress=false;  
			  if(result.data.NumBl=='1'){$scope.screendata.showvin=true; $scope.screendata.actbtn='teal darken-2'; 
			  $scope.screendata.vinlib=result.data.Lib;}
			  if(result.data.NumBl=='-1'){ $scope.screendata.message= "Aucun connaissement trouvé.";}
		 	  if(result.data.NumBl=='-2'){$scope.screendata.message="Données non consultables. Veuillez contacter nos bureaux";}
					
			}) 
			.error(function(err){$scope.screendata.progress=false; $scope.screendata.message='Echec de la demande.';});
			  
		}
		
		if($scope.md.searchvin!=undefined && $scope.screendata.showvin==true)
		{
			$scope.screendata.progress=true; var req=vsom.Vin($scope.md.searchbl,$scope.md.searchvin+''+$scope.screendata.vinlib); 
			req.success(function(result) {   
				$scope.screendata.progress=false; 
				if(result.data.Lib=='1' && result.data.Vehicules.length==0){
					$scope.screendata.message= "Il n'existe plus de chassis libre pour une quotation"; return;
				}
				if(result.data.Lib=='1' && result.data.Vehicules.length>0)
				{ 
				$scope.screendata.progress=true;
				 
				var req=vsom.hack($scope.md.searchbl,$scope.md.searchvin+''+$scope.screendata.vinlib,JSON.stringify(result.data)); 
				req.success(function(res) { 
					console.log(res);
		 	 	if(res.data.status==1){ /*window.location.href=result.data.lnk;*/$scope.screendata.showvin=false;
		 	 	var rm=vsom.inside(res.data.tkey,res.data.tkeyusr);  console.log('envoie token');
		 	 	rm.success(function(resul){ console.log(resul);  $scope.screendata.progress=false; 
		 	 		if(resul.status==200){window.location.href=res.data.lnk;}else{$scope.screendata.message='La demande a échoué.';}})
		 	 	.error(function(er){  $scope.screendata.progress=false;});
		 	 	}
		 	 	if(res.data.status==2){$scope.screendata.message='Compte indisponible'; $scope.screendata.progress=false;}
		 	 	if(res.data.status==0){$scope.screendata.message='Identifiant / mot de passe incohérent'; $scope.screendata.progress=false;}
		 	 	if(res.data.status==3){$scope.screendata.message='La négotiation du canal avec le serveur à échoué'; $scope.screendata.progress=false;}
		    	}).error(function(err){$scope.screendata.progress=false;$scope.screendata.message='Demande non effectuée!';});
				
				}
				
			  	if(result.data.Lib=='0'){ $scope.screendata.message= "Numéro de chassis invalide.";}
		 	   	
			})
			.error(function(err){$scope.screendata.progress=false; Notification.error({message: 'Echec de la demande.',  title: appname});});
			
			/*if($scope.md.searchvin==$scope.screendata.chassis.substring(0,7))
			{
	       var reqSpec =$http({ url: base_url+"SearchElmt/"+sectionid+"/"+$scope.md.searchbl,  method: "GET", params: {}, headers: {'Content-Type': 'application/x-www-form-urlencoded'} });
		   reqSpec.success(function(data){  if(data.res==1){ $scope.screendata=data; $scope.screendata.actbtn='indigo darken-3'; $scope.md={}; $scope.screendata.chassis=''; $scope.screendata.showvin=false;}
		 	if(data.res==-1) Notification.error({message: "Aucun connaissement trouvé ou vous ne disposez pas les accès.",  title: appname});  
		 	if(data.res==-2){ $scope.screendata.actbtn='indigo darken-3'; $scope.md={}; $scope.screendata.chassis=''; $scope.screendata.showvin=false; Notification.error({message: "les élements de ce connaissement ne sont pas encore disponible",  title: appname});} });
		   reqSpec.error(function(data, status, headers, config) { Notification.error({message: 'echec chargement donnée.',  title: appname});});
		   }
		   else{ Notification.error({message: "Chassis non confirmé.",  title: appname});}
			*/
	   }
	   
	   if($scope.md.searchvin==undefined && $scope.screendata.showvin==true){
	   	Notification.error({message: "Chassis non valide.",  title: appname});
	   }
	};
});