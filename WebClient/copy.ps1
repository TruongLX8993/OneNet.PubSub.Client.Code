$source = '.\*'
$dest = 'E:\Projects\PubSubSystem\OneNet.PubSub.Client.Release\WebClient'
# $exclude = '.ps1'
# Copy-Item $source $dest
# #   -Destination $dest -Exclude $exclude -Recurse

# Copy-Item -Path $source -Destination $dest  -Exclude '.ps*,.git,.vscode' -Recurse -Force

Copy-Item '.\css'  $dest  -Recurse -Force
Copy-Item '.\js'  $dest  -Recurse -Force
Copy-Item '.\index.html'  $dest  -Recurse -Force