<?php

//    if(isset($_POST)){
//        
//        $mobilenum = $_POST['mobile_num'];
//        $message = $_POST['message'];
        
        $url = 'http://203.87.236.231:8080/1/smsmessaging/outbound/42252536/requests';

        $arr_post_body = array("address" => "639205246744", "message" => "Trial message");

        $options = array(
                'http' => array(
                    'header'  => "Content-Type: application/json",
                    'method'  => 'POST',
                    'content' => json_encode($arr_post_body)
                )
            );
            $context  = stream_context_create($options);
            $result = file_get_contents($url, false, $context);
        if ($result === FALSE) { 
            /* Handle error */ 
            echo 'FAIL';
        } else {

            $result = json_decode($result, true);
            if($result['resourceReference'] != NULL) {
                echo 'SUCCESS';
            } else {
                echo 'CANNOT SEND MESSAGE';
            }


        }
//    }

?>