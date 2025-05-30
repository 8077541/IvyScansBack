#!/bin/bash
# Script to run database migrations on Railway

echo "Starting database migration..."

# Wait for database to be ready
sleep 10

# Run migrations
dotnet ef database update --verbose

echo "Database migration completed."
