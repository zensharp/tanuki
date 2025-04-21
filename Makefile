.DEFAULT_GOAL := cq

cq: ## Show all Makefile targets
	dotnet run --project Tanuki -- codequality Tanuki/TestFiles/codequality.json

