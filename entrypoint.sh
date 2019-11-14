echo "installing dotnet ef utility..."
until which dotnet-ef || dotnet tool install --global dotnet-ef --version 3.0.0; do
    sleep 1
done

echo "waiting for persisted grant database..."
until dotnet ef -v database update --context PersistedGrantDbContext; do
    sleep 1
done

echo "waiting for configuration database..."
until dotnet ef -v database update --context ConfigurationDbContext; do
    sleep 1
done

echo "starting winged keys..."
dotnet watch run --urls "https://0.0.0.0:5050" -- --context ConfigurationDbContext
