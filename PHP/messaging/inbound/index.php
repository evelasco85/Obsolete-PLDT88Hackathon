<?php
    
    $url = 'http://203.87.236.231:8080/1/smsmessaging/inbound/registrations/42252536/messages';

    $result = file_get_contents($url, false);

    if ($result === FALSE) { 
        /* Handle error */ 
        echo 'FAIL';
        exit(0);
    } else {
        
        $res = json_decode($result);
        if($res->inboundSMSMessage == NULL){
            echo 'NO MESSAGE';
        } else {
            echo json_encode($res->inboundSMSMessage);
        }
        exit(0);
    }

?>