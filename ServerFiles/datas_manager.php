<?php
$xmlData = file_get_contents('php://input');

// Load the XML
if (file_exists("data.xml")) {
    $xml = simplexml_load_file("data.xml");
} else {
    $xml = new SimpleXMLElement('<Root></Root>');
}

$newXml = simplexml_load_string($xmlData);

// Check if an element with the same UUID already exists
$uuidExists = false;
foreach ($xml->Data as $data) {
    if ((string)$data['uuid'] === (string)$newXml->Data['uuid']) {
        $uuidExists = true;
        // Remove all existing Item elements
        unset($data->Item);
        
        // Copy Item elements from the new XML to the existing Data element
        foreach ($newXml->Data->Item as $item) {
            $newItem = $data->addChild('Item', (string)$item);
            $newItem->addAttribute('Type', (string)$item['Type']);
        }
        break;
    }
}

// If UUID doesn't exist yet, add new data at the end
if (!$uuidExists) {
    $data = $xml->addChild('Data');
    $data->addAttribute('uuid', (string)$newXml->Data['uuid']);
    foreach ($newXml->Data->Item as $item) {
        $newItem = $data->addChild('Item', (string)$item);
        $newItem->addAttribute('Type', (string)$item['Type']);
    }
}

// Convert XML content to UTF-8 and save the modified XML
$xmlString = $xml->asXML();
$xmlStringUtf8 = mb_convert_encoding($xmlString, 'UTF-8', 'HTML-ENTITIES');
file_put_contents('data.xml', $xmlStringUtf8);

// Convert XML to CSV
$csv = fopen('data.csv', 'w');
fwrite($csv, "\xEF\xBB\xBF");
foreach ($xml->Data as $data) {
    $uuid = (string)$data['uuid'];
    $rowData = array($uuid);

    foreach ($data->Item as $item) {
        $type = (string)$item['Type'];
        $value = trim((string)$item);
        if ($type === "System.DateTime") {
            $value = date('Y-m-d H:i:s', strtotime($value));
        }
        $rowData[] = $value;
    }

    fputcsv($csv, $rowData, ';');
}
fclose($csv);

echo "Saving xml and csv files completed !";

?>