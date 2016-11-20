<?php

    if(isset($_POST)){
        
        $mobilenum = $_POST['mobile_num'];
        $message = $_POST['message'];
        
        $arr_post_body = array(
            "message_type" => "SEND",
            "mobile_number" => $mobilenum,
            "shortcode" => "29290479",
            "message_id" => md5(uniqid(rand(), true)),
            "message" => $message,
            "client_id" => "8309e5ede730f5a49b31bcc64c00aaa9c1cc3c34b8866983c3d3d960043cf0bb",
            "secret_key" => "33145b3560e7de4df42daeae15afcde73c92ee71597d4850e7a5189bc245bea1"
        );

        $URL = "https://post.chikka.com/smsapi/request";

        $options = array(
            'http' => array(
                'header'  => "Content-Type: application/json",
                'method'  => 'POST',
                'content' => http_build_query($arr_post_body)
            )
        );
        $context  = stream_context_create($options);
        $result = file_get_contents($URL, false, $context);
        if ($result === FALSE) { 
            /* Handle error */ 
            echo 'FAIL';
            exit(0);
        } else {

            $res = json_decode($result);
            if(strtoupper($res['message']) === 'ACCEPTED') {
                echo 'SUCCESS';
                exit(0);
            } else {
                echo 'FAIL';
                exit(0);
            }

        }
    }

?>