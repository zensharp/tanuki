.DEFAULT_GOAL := cq

cq: ## Show all Makefile targets
	cd Tanuki && dotnet watch -- codequality TestFiles/codeclimate.json

