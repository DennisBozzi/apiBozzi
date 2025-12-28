using Microsoft.AspNetCore.Mvc;

namespace apiBozzi.Configurations.Transaction;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TransactionAttribute() : TypeFilterAttribute(typeof(TransactionFilter));