global using MediatR;
global using FluentValidation;
global using NairaWallet.Domain.Entities;
global using NairaWallet.Domain.ValueObjects;
global using NairaWallet.Domain.Enums;
global using NairaWallet.Application.Common.Exceptions;
global using NairaWallet.Application.Common.Interfaces;
global using NairaWallet.Application.DTOs;
global using Microsoft.Extensions.Logging;
global using NairaWallet.Application.Common.Interfaces.Repositories;
global using FluentValidation.Results;
global using ValidationException = NairaWallet.Application.Common.Exceptions.ValidationException;
global using IDateTimeProvider = NairaWallet.Application.Common.Interfaces.IDateTimeProvider;
global using NairaWallet.Application.Common.Mappings;
global using static NairaWallet.Application.Common.Interfaces.IPaystackService;






