﻿using System;
using SharpVk.Interop;
using SkiaSharp.Tests;
using Xunit;
using Xunit.Categories;

namespace SkiaSharp.Vulkan.Desktop.Tests
{
	public class VkTests : SKTest
	{
		[Category(GpuCategory)]
		[SkippableFact]
		public void CreateVkContextIsValid()
		{
			var nativeLibrary = new NativeLibrary();

			using (var ctx = CreateVkContext())
			using (var vkInterface = GRVkInterface.Create(
				nativeLibrary.GetProcedureAddress("vkGetInstanceProcAddr"),
				nativeLibrary.GetProcedureAddress("vkGetDeviceProcAddr"),
				(IntPtr)ctx.Instance.RawHandle.ToUInt64(),
				(IntPtr)ctx.Device.RawHandle.ToUInt64(),
				0))
			{
				Assert.NotNull(vkInterface);
				Assert.True(vkInterface.Validate(0));

				using (var grVkBackendContext = GRVkBackendContext.Assemble(
					(IntPtr)ctx.Instance.RawHandle.ToUInt64(),
					(IntPtr)ctx.PhysicalDevice.RawHandle.ToUInt64(),
					(IntPtr)ctx.Device.RawHandle.ToUInt64(),
					(IntPtr)ctx.GraphicsQueue.RawHandle.ToUInt64(),
					ctx.GraphicsFamily,
					0,
					0,
					0,
					vkInterface))
				{
					Assert.NotNull(grVkBackendContext);

					using (var grContext = GRContext.Create(GRBackend.Vulkan, grVkBackendContext))
					{
						Assert.NotNull(grContext);
					}
				}
			}
		}

		[Category(GpuCategory)]
		[SkippableFact]
		public void VkGpuSurfaceIsCreated()
		{
			var nativeLibrary = new NativeLibrary();

			using (var ctx = CreateVkContext())
			using (var vkInterface = GRVkInterface.Create(
				nativeLibrary.GetProcedureAddress("vkGetInstanceProcAddr"),
				nativeLibrary.GetProcedureAddress("vkGetDeviceProcAddr"),
				(IntPtr)ctx.Instance.RawHandle.ToUInt64(),
				(IntPtr)ctx.Device.RawHandle.ToUInt64(),
				0))
			using (var grVkBackendContext = GRVkBackendContext.Assemble(
				(IntPtr)ctx.Instance.RawHandle.ToUInt64(),
				(IntPtr)ctx.PhysicalDevice.RawHandle.ToUInt64(),
				(IntPtr)ctx.Device.RawHandle.ToUInt64(),
				(IntPtr)ctx.GraphicsQueue.RawHandle.ToUInt64(),
				ctx.GraphicsFamily,
				0,
				0,
				0,
				vkInterface))
			using (var grContext = GRContext.Create(GRBackend.Vulkan, grVkBackendContext))
			using (var surface = SKSurface.Create(grContext, true, new SKImageInfo(100, 100)))
			{
				Assert.NotNull(surface);

				var canvas = surface.Canvas;
				Assert.NotNull(canvas);

				canvas.Clear(SKColors.Transparent);
			}
		}

		private VkContext CreateVkContext()
		{
			try
			{
				if (!IsWindows)
				{
					throw new PlatformNotSupportedException();
				}

				return new Win32VkContext();
			}
			catch (Exception ex)
			{
				throw new SkipException("Unable to create Vulkan context: " + ex.Message);
			}
		}
	}
}
