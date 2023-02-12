echo checklist.txt
type "e:\Zaloha\Projects\Playnite\FilterMaster\checklist.txt"
pause
copy /Y "e:\Zaloha\Projects\Playnite\FilterMaster\extension.yaml" "E:\Zaloha\Projects\Playnite\FilterMaster\bin\Debug\extension.yaml"
d:\Playnite\Toolbox.exe pack "E:\Zaloha\Projects\Playnite\FilterMaster\bin\Debug" "E:\Zaloha\Projects\Playnite"
pause
explorer "E:\Zaloha\Projects\Playnite"
explorer "https://github.com/Gerren/Playnite-FilterMaster/releases/new"
